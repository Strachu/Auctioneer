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
				Id               = auction.Id,
				Title            = auction.Title,
				Description      = auction.Description,
				Status           = auction.Status,
				EndDate          = auction.EndDate,
				IsBiddingEnabled = auction.IsBiddingEnabled,
				BestOffer        = auction.BestOffer != null ? auction.BestOffer.Money : null,
				MinPrice         = auction.MinimumPrice,
				MinAllowedBid    = auction.MinBidAllowed,
				MaxAllowedBid    = auction.IsBuyoutEnabled ? auction.BuyoutPrice.Amount : (decimal?)null,
				IsBuyoutEnabled  = auction.IsBuyoutEnabled,
				BuyoutPrice      = auction.BuyoutPrice,
				SellerUserName   = auction.Seller.UserName,
				BuyerUserName    = (auction.Buyer != null) ? auction.Buyer.UserName : null,
				Photos           = photoUrls.Select(x => new AuctionShowViewModel.Photo { Url = x })
			};
		}
	}
}