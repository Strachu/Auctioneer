using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Resources.Auction.Buy;

namespace Auctioneer.Presentation.Models
{
	public class AuctionBuyViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		[DataType(DataType.Currency)]
		public decimal Price { get; set; }
	}
}