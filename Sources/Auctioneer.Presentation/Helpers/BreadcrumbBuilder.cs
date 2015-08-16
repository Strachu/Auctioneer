using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;

using Auctioneer.Presentation.Infrastructure;
using Auctioneer.Presentation.Models;

using Lang = Auctioneer.Resources.Shared.Layout;

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

		public IBreadcrumbBuilder WithCategoryHierarchy(int leafCategoryId, string searchString)
		{
			var categoryHierarchy = mCategoryService.GetCategoryHierarchy(leafCategoryId).Result;

			foreach(var category in categoryHierarchy)
			{
				mItems.Add(new BreadcrumbViewModel.Item
				{
					Name      = category.Name,
					TargetUrl = mUrlHelper.Action(controllerName: "Category", actionName: "Index", routeValues: new
					{
						id           = category.Id,
						slug         = SlugGenerator.SlugFromTitle(category.Name),
						searchString = searchString
					})
				});
			}

			return this;
		}

		public IBreadcrumbBuilder WithAuctionLink(Auction auction)
		{
			mItems.Add(new BreadcrumbViewModel.Item
			{
				Name      = auction.Title,
				TargetUrl = mUrlHelper.Action(controllerName: "Auction", actionName: "Show", routeValues: new
				{
					id   = auction.Id,
					slug = SlugGenerator.SlugFromTitle(auction.Title)
				})
			});

			return this;		
		}

		public IBreadcrumbBuilder WithSearchResults(string searchString)
		{
			mItems.Add(new BreadcrumbViewModel.Item
			{
				Name      = String.Format(Lang.Breadcrumb_SearchingFor, searchString),
				TargetUrl = mUrlHelper.Action()
			});

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