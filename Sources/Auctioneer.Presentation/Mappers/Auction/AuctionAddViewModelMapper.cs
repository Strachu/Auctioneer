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
		public static Auction ToAuction(AuctionAddViewModel viewModel)
		{
			Contract.Requires(viewModel != null);

			return new Auction
			{
				Title        = viewModel.Title,
				Description  = viewModel.Description,
				CreationDate = DateTime.Now,
				EndDate      = DateTime.Now.AddDays(viewModel.DaysToEnd),
				CategoryId   = viewModel.CategoryId,
				Price        = viewModel.Price.Value,
				PhotoCount   = viewModel.Photos.Count(),
			};
		}
	}
}