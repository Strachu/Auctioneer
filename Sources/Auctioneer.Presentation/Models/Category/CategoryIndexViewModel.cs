using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Models
{
	public class CategoryIndexViewModel
	{
		public CategoryListViewModel Category { get; set; }
		public IEnumerable<AuctionViewModel> Auctions { get; set; }
	}
}