using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

using PagedList;

namespace Auctioneer.Presentation.Mappers.Category
{
	public class CategoryIndexViewModelMapper
	{
		public static CategoryIndexViewModel FromAuctions(IPagedList<Auction> auctions,
		                                                  AuctionSortOrder currentSortOrder,
		                                                  int? categoryId = null)
		{
			Contract.Requires(auctions != null);

			return new CategoryIndexViewModel
			{
				CategoryId       = categoryId,
				Auctions         = new StaticPagedList<AuctionViewModel>(auctions.Select(AuctionViewModelMapper.FromAuction),
				                                                         auctions),
				CurrentSortOrder = currentSortOrder,
			};
		}
	}
}