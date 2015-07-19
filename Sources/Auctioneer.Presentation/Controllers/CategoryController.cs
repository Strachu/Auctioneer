using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic;
using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategoryService mCategoryService;

		public CategoryController()
		{
			mCategoryService = new CategoryService(new AuctioneerDbContext());
		}

		public CategoryController(ICategoryService categoryService)
		{
			mCategoryService = categoryService;
		}

		public async Task<ActionResult> Index(int? id)
		{
			IEnumerable<Category> categories;

			if(id != null)
			{
				var category = await mCategoryService.GetCategoryById(id.Value);

				categories = category.SubCategories;
			}
			else
			{
				categories = await mCategoryService.GetTopLevelCategories();
			}

			categories = categories.OrderBy(x => x.Name);

			var viewModels = categories.Select(x => new CategoryViewModel
			{
				Id   = x.Id,
				Name = x.Name
			});

			return View(viewModels);
		}
	}
}