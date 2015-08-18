using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Auctioneer.Logic.ValueTypes;

namespace Auctioneer.Presentation.Models
{
	public class AuctionViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan TimeTillEnd { get; set; }

		[DataType(DataType.Currency)]
		public Money BuyoutPrice { get; set; }

		[DataType(DataType.Currency)]
		public Money BestBid { get; set; }

		public string Slug { get; set; }
	}
}