using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Models
{
	public class AuctionViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public TimeSpan TimeTillEnd { get; set; }

		public decimal Price { get; set; }
	}
}