using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;

using PagedList;

using Lang = Auctioneer.Presentation.Resources.Account.MyAuctions;

namespace Auctioneer.Presentation.Models.Account
{
	public class AccountMyAuctionsViewModel
	{
		public IPagedList<Item>    Auctions { get; set; }

		public TimeSpan            CreatedIn { get; set; }
		public AuctionStatusFilter CurrentStatusFilter { get; set; }
		public string              TitleFilter { get; set; }

		public class Item
		{
			[Display(Name = "Title", ResourceType = typeof(Lang))]
			public string Title { get; set; }

			[Display(Name = "CategoryName", ResourceType = typeof(Lang))]
			public string CategoryName { get; set; }

			[DataType(DataType.Time)]
			[Display(Name = "TimeTillEnd", ResourceType = typeof(Lang))]
			public TimeSpan TimeTillEnd { get; set; }

			[DataType(DataType.Currency)]
			[Display(Name = "Price", ResourceType = typeof(Lang))]
			public decimal Price { get; set; }

			public int Id { get; set; }
			public int CategoryId { get; set; }

			public bool Expired
			{
				get { return TimeTillEnd.TotalSeconds <= 0; }
			}

			public bool CanBeRemoved
			{
				get { return !Expired; }
			}
		}
	}
}