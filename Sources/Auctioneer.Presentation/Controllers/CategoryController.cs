using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic;
using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Models;

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

			var viewModels = new CategoryIndexViewModel
			{
				Category = new CategoryListViewModel
				{
					Categories = categories.Select(x => new CategoryListViewModel.Category
					{
						Id           = x.Id,
						Name         = x.Name,
						AuctionCount = x.AuctionCount
					}),
				},

				Auctions = auctions.Select(x => new AuctionViewModel
				{
					Id          = x.Id,
					Title       = x.Title,
					Price       = x.Price,
					TimeTillEnd = x.EndDate - DateTime.Now
				})
			};

			return View(viewModels);
		}

		[ChildActionOnly]
		public PartialViewResult TopCategories()
		{
			var categories = mCategoryService.GetTopLevelCategories().Result;

			var viewModels = new CategoryListViewModel
			{
				Categories = categories.Select(x => new CategoryListViewModel.Category
				{
					Id           = x.Id,
					Name         = x.Name,
					AuctionCount = x.AuctionCount
				})
			};

			return PartialView("_List", viewModels);
		}
	}
}