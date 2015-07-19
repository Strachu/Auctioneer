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

		public async Task<ActionResult> Index(int? id)
		{
			IEnumerable<Category> categories;
			IEnumerable<Auction>  auctions;

			if(id != null)
			{
				var category = await mCategoryService.GetCategoryById(id.Value);

				categories = category.SubCategories;

				auctions = await mAuctionService.GetActiveAuctionsInCategory(id.Value);
			}
			else
			{
				categories = await mCategoryService.GetTopLevelCategories();
				auctions   = new Auction[0]; // TODO
			}

			categories = categories.OrderBy(x => x.Name);

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
	}
}