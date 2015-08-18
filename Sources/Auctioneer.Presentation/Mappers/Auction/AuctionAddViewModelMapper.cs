using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.ValueTypes;
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
				BuyoutPrice  = new Money(viewModel.Price.Amount, viewModel.Price.Currency),
				PhotoCount   = viewModel.Photos.Count(),
				SellerId     = sellerId
			};
		}
	}
}