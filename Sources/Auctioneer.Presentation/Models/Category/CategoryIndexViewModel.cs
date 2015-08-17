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
		public int? CategoryId { get; set; }

		public IPagedList<AuctionViewModel> Auctions { get; set; }

		public AuctionSortOrder CurrentSortOrder { get; set; }
	}
}