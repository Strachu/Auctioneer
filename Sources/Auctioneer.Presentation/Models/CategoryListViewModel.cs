using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Models
{
	public class CategoryListViewModel
	{
		public IEnumerable<CategoryViewModel> Subcategories { get; set; }
		public IEnumerable<AuctionViewModel> Auctions { get; set; }
	}
}