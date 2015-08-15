using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.ValueTypes;

using Lang = Auctioneer.Resources.Auction.Show;

namespace Auctioneer.Presentation.Models
{
	public class AuctionShowViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		[DataType(DataType.Html)]
		public string Description { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }

		public AuctionStatus Status { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan TimeTillEnd
		{
			get { return EndDate - DateTime.Now; }
		}

		[DataType(DataType.Currency)]
		public Money Price { get; set; }

		[Display(Name = "SellerUserName", ResourceType = typeof(Lang))]
		public string SellerUserName { get; set; }

		[Display(Name = "BuyerUserName", ResourceType = typeof(Lang))]
		public string BuyerUserName { get; set; }

		public IEnumerable<Photo> Photos { get; set; }

		public class Photo
		{
			[DataType(DataType.ImageUrl)]
			public string Url { get; set; }
		}

		public bool CanBeBought { get; set; }
		public bool CanBeRemoved { get; set; }
	}
}