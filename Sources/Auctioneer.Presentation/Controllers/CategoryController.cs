using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Mappers.Category;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Infrastructure.Http;

namespace Auctioneer.Presentation.Controllers
{
	public class CategoryController : Controller
	{
		private const string COOKIE_SORT_ORDER_KEY = "sortOrder";
		private const string COOKIE_PAGE_SIZE_KEY  = "pageSize";

		private readonly ICategoryService   mCategoryService;
		private readonly IAuctionService    mAuctionService;
		private readonly IBreadcrumbBuilder mBreadcrumbBuilder;
		private readonly HttpRequestBase    mRequest;
		private readonly HttpResponseBase   mResponse;

		public CategoryController(ICategoryService categoryService,
		                          IAuctionService auctionService,
		                          IBreadcrumbBuilder breadcrumbBuilder,
		                          HttpRequestBase request,
		                          HttpResponseBase response)
		{
			mCategoryService   = categoryService;
			mAuctionService    = auctionService;
			mBreadcrumbBuilder = breadcrumbBuilder;
			mRequest           = request;
			mResponse          = response;
		}

		[ChildActionOnly]
		public PartialViewResult Categories(int? parentCategoryId = null, string searchString = null)
		{
			var categories = mCategoryService.GetCategoriesAlongWithAuctionCount(parentCategoryId, searchString).Result;

			if(!String.IsNullOrWhiteSpace(searchString))
			{
				categories = categories.Where(x => x.AuctionCount > 0);
			}

			var viewModel = CategoryListViewModelMapper.FromCategoriesWithAuctionCount(categories, searchString);
			return PartialView("_List", viewModel);
		}

		public async Task<ActionResult> All(string searchString = null, AuctionSortOrder? sortOrder = null, int page = 1,
		                                    int? pageSize = null)
		{
			InitializeAuctionListingParameters(ref pageSize, ref sortOrder);

			var auctions  = await mAuctionService.GetAllActiveAuctions(searchString, sortOrder.Value, page, pageSize.Value);				
			var viewModel = CategoryIndexViewModelMapper.FromAuctions(auctions, currentSortOrder: sortOrder.Value);

			return View("Index", viewModel);
		}

		[Route("{lang:language}/Category/{id:int?}/{slug?}", Order = 1)]
		[Route("Category/{id:int?}/{slug?}", Order = 2)]
		public async Task<ActionResult> Index(int id, string searchString = null, AuctionSortOrder? sortOrder = null,
		                                      int page = 1, int? pageSize = null)
		{
			InitializeAuctionListingParameters(ref pageSize, ref sortOrder);

			var auctions  = await mAuctionService.GetActiveAuctionsInCategory(id, searchString, sortOrder.Value, page,
			                                                                  pageSize.Value);
			var viewModel = CategoryIndexViewModelMapper.FromAuctions(auctions, sortOrder.Value, categoryId: id);

			return View(viewModel);
		}

		private void InitializeAuctionListingParameters(ref int? pageSize, ref AuctionSortOrder? sortOrder)
		{
			mResponse.SaveToCookieIfNotNull(COOKIE_PAGE_SIZE_KEY,  pageSize);
			mResponse.SaveToCookieIfNotNull(COOKIE_SORT_ORDER_KEY, sortOrder);

			pageSize  = pageSize  ?? mRequest.ReadIntFromCookie(COOKIE_PAGE_SIZE_KEY)                     ?? 20;
			sortOrder = sortOrder ?? mRequest.ReadEnumFromCookie<AuctionSortOrder>(COOKIE_SORT_ORDER_KEY) ?? AuctionSortOrder.EndDateAscending;

			pageSize  = Math.Min(pageSize.Value, 50);
		}

		public ActionResult Search(int? id, string searchString)
		{
			if(id != null)
			{
				return RedirectToAction("Index", routeValues: new { id, searchString });
			}
			else
			{
				return RedirectToAction("All", routeValues: new { searchString });
			}
		}

		[ChildActionOnly]
		public ActionResult Breadcrumb(int? id = null, string searchString = null)
		{
			var breadcrumb = mBreadcrumbBuilder.WithHomepageLink();
			
			if(searchString != null)
			{
				breadcrumb.WithSearchResults(searchString);
			}

			if(id != null)
			{
				breadcrumb.WithCategoryHierarchy(id.Value, searchString);
			}
			
			return PartialView("_Breadcrumb", breadcrumb.Build());
		}
	}
}