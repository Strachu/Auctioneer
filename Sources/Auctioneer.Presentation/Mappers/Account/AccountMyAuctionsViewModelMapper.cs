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
		public static AccountMyAuctionsViewModel FromAuctions(IPagedList<Auction> auctions, int createdInDays)
		{
			Contract.Requires(auctions != null);

			var items = auctions.Select(x => new AccountMyAuctionsViewModel.Item
			{
				Id           = x.Id,
				Title        = x.Title,
				CategoryName = x.Category.Name,
				CategoryId   = x.Category.Id,
				TimeTillEnd  = x.EndDate - DateTime.Now,
				Price        = x.Price
			});

			return new AccountMyAuctionsViewModel
			{
				Auctions  = new StaticPagedList<AccountMyAuctionsViewModel.Item>(items, auctions),
				CreatedIn = TimeSpan.FromDays(createdInDays)
			};
		}
	}
}