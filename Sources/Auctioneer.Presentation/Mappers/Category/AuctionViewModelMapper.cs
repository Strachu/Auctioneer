using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Mappers.Category
{
	public class AuctionViewModelMapper
	{
		public static AuctionViewModel FromAuction(Auction auction)
		{
			Contract.Requires(auction != null);

			return new AuctionViewModel
			{
				Id          = auction.Id,
				Title       = auction.Title,
				Price       = auction.Price,
				TimeTillEnd = auction.EndDate - DateTime.Now
			};
		}
	}
}