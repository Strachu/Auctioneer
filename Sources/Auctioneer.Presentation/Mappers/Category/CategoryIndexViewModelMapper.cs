using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Mappers.Category
{
	public class CategoryIndexViewModelMapper
	{
		public static CategoryIndexViewModel FromCategoriesAndAuctions(IEnumerable<Logic.Categories.Category> categories,
		                                                               IEnumerable<Auction> auctions)
		{
			Contract.Requires(categories != null);
			Contract.Requires(auctions != null);

			return new CategoryIndexViewModel
			{
				Category = CategoryListViewModelMapper.FromCategories(categories),
				Auctions = auctions.Select(AuctionViewModelMapper.FromAuction)
			};
		}
	}
}