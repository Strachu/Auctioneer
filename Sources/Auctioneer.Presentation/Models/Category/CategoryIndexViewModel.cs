using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Auctions;

using PagedList;

namespace Auctioneer.Presentation.Models
{
	public class CategoryIndexViewModel
	{
		public AuctionSortOrder CurrentSortOrder { get; set; }

		public CategoryListViewModel Category { get; set; }
		public IPagedList<AuctionViewModel> Auctions { get; set; }
	}
}