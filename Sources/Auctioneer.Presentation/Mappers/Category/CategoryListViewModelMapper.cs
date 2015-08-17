using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Mappers.Category
{
	public class CategoryListViewModelMapper
	{
		public static CategoryListViewModel FromCategoriesWithAuctionCount(IEnumerable<CategoryAuctionCountPair> categories,
		                                                                   string searchString)
		{
			Contract.Requires(categories != null);

			return new CategoryListViewModel
			{
				Categories = categories.Select(x => new CategoryListViewModel.Category
				{
					Id           = x.Category.Id,
					Name         = x.Category.Name,
					AuctionCount = x.AuctionCount
				}),
				SearchString = searchString
			};
		}
	}
}