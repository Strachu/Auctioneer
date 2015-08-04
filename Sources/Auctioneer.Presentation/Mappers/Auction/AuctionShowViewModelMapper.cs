using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Mappers
{
	public class AuctionShowViewModelMapper
	{		
		public static AuctionShowViewModel FromAuction(Auction auction, IEnumerable<string> photoUrls)
		{
			Contract.Requires(auction != null);

			return new AuctionShowViewModel
			{
				Title          = auction.Title,
				Description    = auction.Description,
				EndDate        = auction.EndDate,
				Price          = auction.Price,
				SellerUserName = auction.Seller.UserName,
				Photos         = photoUrls.Select(x => new AuctionShowViewModel.Photo
				{
					Url = x
				})
			};
		}
	}
}