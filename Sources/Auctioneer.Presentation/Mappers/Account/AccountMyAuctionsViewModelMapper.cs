using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models.Account;

using PagedList;

namespace Auctioneer.Presentation.Mappers.Account
{
	public class AccountMyAuctionsViewModelMapper
	{
		public static AccountMyAuctionsViewModel FromAuctions(IPagedList<Auction> auctions,
		                                                      string titleFilter,
		                                                      int createdInDays,
		                                                      AuctionStatusFilter currentStatusFilter)
		{
			Contract.Requires(auctions != null);

			var items = auctions.Select(x => new AccountMyAuctionsViewModel.Item
			{
				Id           = x.Id,
				Title        = x.Title,
				CategoryName = x.Category.Name,
				CategoryId   = x.Category.Id,
				Status       = x.Status,
				TimeTillEnd  = x.EndDate - DateTime.Now,
				BuyoutPrice  = x.BuyoutPrice,
				BestOffer    = x.BestOffer != null ? x.BestOffer.Money : null
			});

			return new AccountMyAuctionsViewModel
			{
				Auctions            = new StaticPagedList<AccountMyAuctionsViewModel.Item>(items, auctions),
				TitleFilter         = titleFilter,
				CreatedIn           = TimeSpan.FromDays(createdInDays),
				CurrentStatusFilter = currentStatusFilter
			};
		}
	}
}