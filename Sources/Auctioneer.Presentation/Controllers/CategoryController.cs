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

using PagedList;

namespace Auctioneer.Presentation.Controllers
{
	public class CategoryController : Controller
	{
		private const string COOKIE_SORT_ORDER_KEY = "sortOrder";
		private const string COOKIE_PAGE_SIZE_KEY  = "pageSize";
		private const int    DEFAULT_PAGE_SIZE     = 20;
		private const int    MAX_PAGE_SIZE         = 50;

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

		// TODO Is it better to extract searching to a different method?
		[Route("{lang:language}/Category/{id:int?}/{slug?}", Order = 1)]
		[Route("Category/{id:int?}/{slug?}", Order = 2)]
		public async Task<ActionResult> Index(int? id, string searchString = null, int page = 1, int? pageSize = null,
		                                      AuctionSortOrder? sortOrder = null)
		{
			mResponse.SaveToCookieIfNotNull(COOKIE_PAGE_SIZE_KEY,  pageSize);
			mResponse.SaveToCookieIfNotNull(COOKIE_SORT_ORDER_KEY, sortOrder);

			pageSize  = pageSize  ?? mRequest.ReadIntFromCookie(COOKIE_PAGE_SIZE_KEY)                     ?? DEFAULT_PAGE_SIZE;
			sortOrder = sortOrder ?? mRequest.ReadEnumFromCookie<AuctionSortOrder>(COOKIE_SORT_ORDER_KEY) ?? AuctionSortOrder.EndDateAscending;

			pageSize  = Math.Min(pageSize.Value, MAX_PAGE_SIZE);

			IPagedList<Auction> auctions;

			// TODO should this condition be moved to a service?
			if(id != null)
			{
				auctions = await mAuctionService.GetActiveAuctionsInCategory(id.Value, searchString, sortOrder.Value,
				                                                             page, pageSize.Value);
			}
			else
			{
				auctions = await mAuctionService.GetAllActiveAuctions(searchString, sortOrder.Value, page, pageSize.Value);				
			}

			var viewModel = CategoryIndexViewModelMapper.FromAuctions(auctions, id, sortOrder.Value);
			return View(viewModel);
		}

		public ActionResult Search(int? id, string searchString)
		{
			return RedirectToAction("Index", routeValues: new { id, searchString });
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