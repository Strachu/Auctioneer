using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Utils;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Helpers
{
	public class BreadcrumbBuilder : IBreadcrumbBuilder
	{
		private readonly UrlHelper                      mUrlHelper;
		private readonly ICategoryService               mCategoryService;

		private readonly List<BreadcrumbViewModel.Item> mItems = new List<BreadcrumbViewModel.Item>();

		public BreadcrumbBuilder(UrlHelper urlHelper, ICategoryService categoryService)
		{
			Contract.Requires(urlHelper != null);
			Contract.Requires(categoryService != null);

			mUrlHelper       = urlHelper;
			mCategoryService = categoryService;
		}

		public IBreadcrumbBuilder WithHomepageLink()
		{
			mItems.Add(new BreadcrumbViewModel.Item
			{
				Name      = "Auctioneer",
				TargetUrl = mUrlHelper.Action(controllerName: "Home", actionName: "Index")
			});

			return this;
		}

		public IBreadcrumbBuilder WithCategoryHierarchy(int leafCategoryId)
		{
			var categoryHierarchy = mCategoryService.GetCategoryHierarchy(leafCategoryId).Result;

			foreach(var category in categoryHierarchy)
			{
				mItems.Add(new BreadcrumbViewModel.Item
				{
					Name      = category.Name,
					TargetUrl = mUrlHelper.Action(controllerName: "Category", actionName: "Index", routeValues: new
					{
						id   = category.Id,
						slug = SlugGenerator.SlugFromTitle(category.Name)
					})
				});
			}

			return this;
		}

		public BreadcrumbViewModel Build()
		{
			return new BreadcrumbViewModel
			{
				Items = mItems.ToList()
			};
		}
	}
}