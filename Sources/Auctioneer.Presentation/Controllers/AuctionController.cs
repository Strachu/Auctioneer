﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Currencies;
using Auctioneer.Presentation.Emails;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Infrastructure.Validation;
using Auctioneer.Presentation.Mappers;
using Auctioneer.Presentation.Models;
using Auctioneer.Presentation.Models.Shared;

using Microsoft.AspNet.Identity;

using DevTrends.MvcDonutCaching;

using Postal;

namespace Auctioneer.Presentation.Controllers
{
	public class AuctionController : Controller
	{
		private readonly IAuctionService    mAuctionService;
		private readonly ICategoryService   mCategoryService;
		private readonly ICurrencyService   mCurrencyService;
		private readonly IBreadcrumbBuilder mBreadcrumbBuilder;

		public AuctionController(IAuctionService auctionService,
		                         ICategoryService categoryService,
		                         ICurrencyService currencyService,
		                         IBreadcrumbBuilder breadcrumbBuilder)
		{
			Contract.Requires(auctionService != null);
			Contract.Requires(categoryService != null);
			Contract.Requires(currencyService != null);
			Contract.Requires(breadcrumbBuilder != null);

			mAuctionService = auctionService;
			mCategoryService = categoryService;
			mCurrencyService = currencyService;
			mBreadcrumbBuilder = breadcrumbBuilder;
		}

		[Route("{lang:language}/Auction/{id:int}/{slug?}", Order = 1)]
		[Route("Auction/{id:int}/{slug?}", Order = 2)]
		public async Task<ActionResult> Show(int id)
		{
			var auction = await mAuctionService.GetById(id);

			var photoUrls = new List<string>(auction.PhotoCount);
			for(int photoIndex = 0; photoIndex < auction.PhotoCount; ++photoIndex)
			{
				photoUrls.Add(Url.AuctionPhoto(id, photoIndex));
			}

			var viewModel = AuctionShowViewModelMapper.FromAuction(auction, photoUrls);

			viewModel.CanBeBought  = mAuctionService.CanBeBought(auction, User.Identity.GetUserId());
			viewModel.CanBeRemoved = auction.Status == AuctionStatus.Active && auction.SellerId == User.Identity.GetUserId(); // TODO move it to a service?

			return View(viewModel);
		}

		[ChildActionOnly]
		public ActionResult Breadcrumb(int id)
		{
			var auction    = mAuctionService.GetById(id).Result;
			var breadcrumb = mBreadcrumbBuilder.WithHomepageLink()
			                                   .WithCategoryHierarchy(auction.CategoryId)
			                                   .WithAuctionLink(auction)
			                                   .Build();

			return PartialView("_Breadcrumb", breadcrumb);
		}

		[Authorize]
		[DonutOutputCache(Duration = 7200)]
		public async Task<ActionResult> Add()
		{
			var viewModel = new AuctionAddViewModel();

			await PopulateAvailableCategoryList(viewModel);
			await PopulateAvailableCurrencyList(viewModel.Price);
			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Add(AuctionAddViewModel input)
		{
			if(!ModelState.IsValid)
			{
				await PopulateAvailableCategoryList(input);
				await PopulateAvailableCurrencyList(input.Price);
				return View(input);
			}

			var newAuction = AuctionAddViewModelMapper.ToAuction(input, sellerId: User.Identity.GetUserId());

			await mAuctionService.AddAuction(newAuction, input.Photos.Select(x => x.InputStream));

			return RedirectToAction("Show", new { id = newAuction.Id });
		}

		private async Task PopulateAvailableCategoryList(AuctionAddViewModel viewModel)
		{
			var nbsp       = '\xA0';
			var categories = await mCategoryService.GetAllCategoriesWithHierarchyLevel();

			viewModel.AvailableCategories = categories.Select(x => new SelectListItem
			{
				Text  = new string(nbsp, count: x.HierarchyDepth * 3) + x.Category.Name,
				Value = x.Category.Id.ToString()
			});
		}

		private async Task PopulateAvailableCurrencyList(MoneyEditViewModel viewModel)
		{
			var currencies = await mCurrencyService.GetAllCurrencies();

			viewModel.AvailableCurrencies = currencies.Select(x => new SelectListItem
			{
				Text     = x.Symbol,
				Value    = x.Symbol,
				Selected = x.Symbol == Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol
			});
		}

		[Authorize]
		public async Task<ActionResult> Delete(int id)
		{
			var auction   = await mAuctionService.GetById(id);
			var viewModel = new AuctionDeleteViewModel { Id = id, Title = auction.Title };

			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeletePost(int id)
		{
			await mAuctionService.RemoveAuctions(new int[] { id }, User.Identity.GetUserId(),
			                                     new ValidationErrorNotifierAdapter(ModelState));

			if(!ModelState.IsValid)
				return View("Error");

			// TODO This should display some confirmation that the auction was deleted and redirect to previous page
			return RedirectToAction(controllerName: "Home", actionName: "Index");
		}

		[Authorize]
		public async Task<ActionResult> Buy(int id)
		{
			var auction   = await mAuctionService.GetById(id);
			var viewModel = new AuctionBuyViewModel { Id = id, Title = auction.Title, Price = auction.Price };

			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ActionName("Buy")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BuyPost(int id)
		{
			await mAuctionService.Buy(id, User.Identity.GetUserId(), new ValidationErrorNotifierAdapter(ModelState));

			if(!ModelState.IsValid)
				return View("Error");

			return RedirectToAction("OrderConfirmed");
		}

		public ActionResult OrderConfirmed()
		{
			return View();
		}
	}
}