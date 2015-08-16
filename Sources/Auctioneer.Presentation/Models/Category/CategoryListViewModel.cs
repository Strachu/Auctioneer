using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Models
{
	public class CategoryListViewModel
	{
		public IEnumerable<Category> Categories { get; set; }

		public string SearchString { get; set; }

		public class Category
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public int AuctionCount { get; set; }
		}
	}
}