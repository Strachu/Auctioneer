using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic;
using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Mappers;
using Auctioneer.Presentation.Models;

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

		[OutputCache(Duration = 7200)]
		public async Task<ActionResult> Add()
		{
			var viewModel = new AuctionAddViewModel();

			await PopulateAvailableCategoryList(viewModel);
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Add(AuctionAddViewModel input)
		{
			if(ModelState.IsValid)
			{
				var newAuction = AuctionAddViewModelMapper.ToAuction(input);

				using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					await mAuctionService.AddAuction(newAuction);
					await mAuctionService.StoreAuctionPhotos(newAuction.Id, input.Photos.Select(x => x.InputStream));

					transaction.Complete();
				}

				return RedirectToAction("Show", new { id = newAuction.Id });
			}

			await PopulateAvailableCategoryList(input);
			return View(input);
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
	}
}