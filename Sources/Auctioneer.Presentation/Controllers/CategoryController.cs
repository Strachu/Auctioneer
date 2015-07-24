using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Mappers.Category;

namespace Auctioneer.Presentation.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategoryService mCategoryService;
		private readonly IAuctionService  mAuctionService;

		public CategoryController(ICategoryService categoryService, IAuctionService auctionService)
		{
			mCategoryService = categoryService;
			mAuctionService  = auctionService;
		}

		[Route("Category/{id}/{slug}")]
		public async Task<ActionResult> Index(int id)
		{
			var categories = await mCategoryService.GetSubcategories(parentCategoryId: id);
			var auctions   = await mAuctionService.GetActiveAuctionsInCategory(id);

			return View(CategoryIndexViewModelMapper.FromCategoriesAndAuctions(categories, auctions));
		}

		[ChildActionOnly]
		public PartialViewResult TopCategories()
		{
			var categories = mCategoryService.GetTopLevelCategories().Result;

			return PartialView("_List", CategoryListViewModelMapper.FromCategories(categories));
		}
	}
}