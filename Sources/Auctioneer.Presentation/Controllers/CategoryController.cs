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

		[Route("Category/{id}")]
		public async Task<ActionResult> Index(int id)
		{
			var category = await mCategoryService.GetCategoryById(id);
			var auctions = await mAuctionService.GetActiveAuctionsInCategory(id);

			var categories = category.SubCategories.OrderBy(x => x.Name);

			var viewModels = new CategoryListViewModel
			{
				Subcategories = categories.Select(x => new CategoryViewModel
				{
					Id = x.Id,
					Name = x.Name
				}),

				Auctions = auctions.Select(x => new AuctionViewModel
				{
					Id = x.Id,
					Title = x.Title,
					Price = x.Price,
					TimeTillEnd = DateTime.Now - x.EndDate
				})
			};

			return View(viewModels);
		}

		[ChildActionOnly]
		public PartialViewResult TopCategories()
		{
			var categories = mCategoryService.GetTopLevelCategories().Result;
			categories     = categories.OrderBy(x => x.Name);

			var viewModels = categories.Select(x => new CategoryViewModel
			{
				Id   = x.Id,
				Name = x.Name
			});

			return PartialView("_List", viewModels);
		}
	}
}