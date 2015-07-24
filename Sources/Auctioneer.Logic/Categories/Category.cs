using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Categories
{
	public class Category
	{
		public Category()
		{
			SubCategories = new List<Category>();
		}

		public int Id { get; set; }
		public string Name { get; set; }

		public int AuctionCount { get; set; }

		public int? ParentId { get; set; }
		public virtual Category Parent { get; set; }
		public virtual ICollection<Category> SubCategories { get; set; }

		// Nested set model: http://mikehillyer.com/articles/managing-hierarchical-data-in-mysql/ for fast querying
		// of all auctions in category and its subcategories
		public int Left { get; set; }
		public int Right { get; set; }
	}
}
