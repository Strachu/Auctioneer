using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Presentation.Resources.Auction.Show;

namespace Auctioneer.Presentation.Models
{
	public class AuctionShowViewModel
	{
		public string Title { get; set; }

		[DataType(DataType.Html)]
		public string Description { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan TimeTillEnd
		{
			get { return EndDate - DateTime.Now; }
		}

		[DataType(DataType.Currency)]
		public decimal Price { get; set; }

		[Display(Name = "SellerUserName", ResourceType = typeof(Lang))]
		public string SellerUserName { get; set; }

		public IEnumerable<Photo> Photos { get; set; }

		public class Photo
		{
			[DataType(DataType.ImageUrl)]
			public string Url { get; set; }
		}
	}
}