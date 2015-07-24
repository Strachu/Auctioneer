using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Mappers.Category
{
	public class CategoryListViewModelMapper
	{
		public static CategoryListViewModel FromCategories(IEnumerable<Logic.Categories.Category> categories)
		{
			Contract.Requires(categories != null);

			return new CategoryListViewModel
			{
				Categories = categories.Select(x => new CategoryListViewModel.Category
				{
					Id           = x.Id,
					Name         = x.Name,
					AuctionCount = x.AuctionCount
				}),
			};
		}
	}
}