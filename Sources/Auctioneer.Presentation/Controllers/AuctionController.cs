using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Infrastructure.Validation;
using Auctioneer.Presentation.Mappers;
using Auctioneer.Presentation.Models;

using Microsoft.AspNet.Identity;

using DevTrends.MvcDonutCaching;

using Lang = Auctioneer.Resources.Auction;

namespace Auctioneer.Presentation.Controllers
{
	public class AuctionController : Controller
	{
		private readonly IAuctionService    mAuctionService;
		private readonly ICategoryService   mCategoryService;
		private readonly IBreadcrumbBuilder mBreadcrumbBuilder;

		public AuctionController(IAuctionService auctionService,
		                         ICategoryService categoryService,
		                         IBreadcrumbBuilder breadcrumbBuilder)
		{
			Contract.Requires(auctionService != null);
			Contract.Requires(categoryService != null);
			Contract.Requires(breadcrumbBuilder != null);

			mAuctionService    = auctionService;
			mCategoryService   = categoryService;
			mBreadcrumbBuilder = breadcrumbBuilder;
		}

		[Route("Auction/{id:int}/{slug?}")]
		public async Task<ActionResult> Show(int id)
		{
			var auction = await mAuctionService.GetById(id);

			var photoUrls = new List<string>(auction.PhotoCount);
			for(int photoIndex = 0; photoIndex < auction.PhotoCount; ++photoIndex)
			{
				photoUrls.Add(Url.AuctionPhoto(id, photoIndex));
			}

			var viewModel = AuctionShowViewModelMapper.FromAuction(auction, photoUrls);

			viewModel.CanBeBought                   = mAuctionService.CanBeBought(auction, User.Identity.GetUserId());
			viewModel.CanBeRemoved                  = await mAuctionService.CanBeRemoved(auction, User.Identity.GetUserId());
			viewModel.CanBeMovedToDifferentCategory = await mAuctionService.CanBeMoved(auction, User.Identity.GetUserId());

			if(viewModel.CanBeMovedToDifferentCategory)
			{
				viewModel.AvailableCategories = await GetAvailableCategoryList(currentCategoryId: auction.CategoryId);
			}

			ViewBag.Message = TempData["ConfirmationMessage"];
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
			var viewModel = new AuctionAddViewModel
			{
				AvailableCategories = await GetAvailableCategoryList()
			};

			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Add(AuctionAddViewModel input)
		{
			if(!ModelState.IsValid)
			{
				input.AvailableCategories = await GetAvailableCategoryList();
				return View(input);
			}

			var newAuction = AuctionAddViewModelMapper.ToAuction(input, sellerId: User.Identity.GetUserId());

			await mAuctionService.AddAuction(newAuction, input.Photos.Select(x => x.InputStream));

			return RedirectToAction("Show", new { id = newAuction.Id });
		}

		private async Task<IEnumerable<SelectListItem>> GetAvailableCategoryList(int? currentCategoryId = null)
		{
			var nbsp       = '\xA0';
			var categories = await mCategoryService.GetAllCategoriesWithHierarchyLevel();

			return categories.Select(x => new SelectListItem
			{
				Text     = new string(nbsp, count: x.HierarchyDepth * 3) + x.Category.Name,
				Value    = x.Category.Id.ToString(),
				Selected = (currentCategoryId.HasValue && x.Category.Id == currentCategoryId)
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

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Move(int id, int categoryId)
		{
			await mAuctionService.MoveAuction(id, categoryId, User.Identity.GetUserId(),
			                                  new ValidationErrorNotifierAdapter(ModelState));

			if(!ModelState.IsValid)
				return View("Error");

			TempData["ConfirmationMessage"] = Lang.Show.AuctionMoved;
			return RedirectToAction("Show", routeValues: new { id });
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