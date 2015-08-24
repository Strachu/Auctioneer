using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Mappers
{
	public class AuctionAddViewModelMapper
	{		
		public static Auction ToAuction(AuctionAddViewModel viewModel, string sellerId)
		{
			Contract.Requires(viewModel != null);

			return new Auction
			{
				Title        = viewModel.Title,
				Description  = viewModel.Description,
				CreationDate = DateTime.Now,
				EndDate      = DateTime.Now.AddDays(viewModel.DaysToEnd),
				CategoryId   = viewModel.CategoryId,
				MinimumPrice = viewModel.IsBiddingEnabled ? viewModel.MinimumBiddingPrice.ToMoney() : null,
				BuyoutPrice  = viewModel.IsBuyoutEnabled  ? viewModel.BuyoutPrice.ToMoney()         : null,
				PhotoCount   = viewModel.Photos.Count(),
				SellerId     = sellerId
			};
		}
	}
}