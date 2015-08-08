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

using DevTrends.MvcDonutCaching;

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

		[Route("Category/{id}/{slug?}")]
		public async Task<ActionResult> Index(int id, int? page, int? pageSize, AuctionSortOrder? sortOrder)
		{
			mResponse.SaveToCookieIfNotNull(COOKIE_PAGE_SIZE_KEY,  pageSize);
			mResponse.SaveToCookieIfNotNull(COOKIE_SORT_ORDER_KEY, sortOrder);

			pageSize  = pageSize  ?? mRequest.ReadIntFromCookie(COOKIE_PAGE_SIZE_KEY)                     ?? DEFAULT_PAGE_SIZE;
			sortOrder = sortOrder ?? mRequest.ReadEnumFromCookie<AuctionSortOrder>(COOKIE_SORT_ORDER_KEY) ?? AuctionSortOrder.EndDateAscending;

			pageSize  = Math.Min(pageSize.Value, MAX_PAGE_SIZE);

			var categories = await mCategoryService.GetSubcategories(parentCategoryId: id);
			var auctions   = await mAuctionService.GetActiveAuctionsInCategory(id, sortOrder.Value, page ?? 1, pageSize.Value);
			var viewModel  = CategoryIndexViewModelMapper.FromCategoriesAndAuctions(categories, auctions, sortOrder.Value);

			return View(viewModel);
		}

		[ChildActionOnly]
		[DonutOutputCache(Duration = 7200)]
		public PartialViewResult TopCategories()
		{
			var categories = mCategoryService.GetTopLevelCategories().Result;
			var viewModel  = CategoryListViewModelMapper.FromCategories(categories);

			return PartialView("_List", viewModel);
		}

		[ChildActionOnly]
		[DonutOutputCache(Duration = Constants.DAY, VaryByParam = "id")]
		public ActionResult Breadcrumb(int id)
		{
			var breadcrumb = mBreadcrumbBuilder.WithHomepageLink()
			                                   .WithCategoryHierarchy(id)
			                                   .Build();

			return PartialView("_Breadcrumb", breadcrumb);
		}
	}
}