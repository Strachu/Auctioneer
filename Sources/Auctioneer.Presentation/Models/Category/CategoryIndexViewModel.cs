using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PagedList;

namespace Auctioneer.Presentation.Models
{
	public class CategoryIndexViewModel
	{
		public CategoryListViewModel Category { get; set; }
		public IPagedList<AuctionViewModel> Auctions { get; set; }
	}
}