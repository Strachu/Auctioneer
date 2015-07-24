using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Mappers.Category;

namespace Auctioneer.Presentation.Controllers
{
	public class CategoryController : Controller
	{
		private const string COOKIE_PAGE_SIZE_KEY = "pageSize";
		private const int    DEFAULT_PAGE_SIZE    = 20;
		private const int    MAX_PAGE_SIZE        = 50;

		private readonly ICategoryService mCategoryService;
		private readonly IAuctionService  mAuctionService;
		private readonly HttpRequestBase  mRequest;
		private readonly HttpResponseBase mResponse;

		public CategoryController(ICategoryService categoryService,
		                          IAuctionService auctionService,
		                          HttpRequestBase request,
		                          HttpResponseBase response)
		{
			mCategoryService = categoryService;
			mAuctionService  = auctionService;
			mRequest         = request;
			mResponse        = response;
		}

		[Route("Category/{id}/{slug}")]
		public async Task<ActionResult> Index(int id, int? page, int? pageSize, AuctionSortOrder? sortOrder)
		{
			if(pageSize == null)
			{
				if(mRequest.Cookies[COOKIE_PAGE_SIZE_KEY] != null)
				{
					pageSize = Int32.Parse(mRequest.Cookies[COOKIE_PAGE_SIZE_KEY].Value);
				}
				else
				{
					pageSize = DEFAULT_PAGE_SIZE;
				}
			}
			else
			{
				mResponse.SetCookie(new HttpCookie(COOKIE_PAGE_SIZE_KEY, pageSize.ToString()));
			}

			pageSize  = Math.Min(pageSize.Value, MAX_PAGE_SIZE);
			sortOrder = sortOrder ?? AuctionSortOrder.EndDateAscending;

			var categories = await mCategoryService.GetSubcategories(parentCategoryId: id);
			var auctions   = await mAuctionService.GetActiveAuctionsInCategory(id, sortOrder.Value, page ?? 1, pageSize.Value);

			return View(CategoryIndexViewModelMapper.FromCategoriesAndAuctions(categories, auctions, sortOrder.Value));
		}

		[ChildActionOnly]
		public PartialViewResult TopCategories()
		{
			var categories = mCategoryService.GetTopLevelCategories().Result;

			return PartialView("_List", CategoryListViewModelMapper.FromCategories(categories));
		}
	}
}