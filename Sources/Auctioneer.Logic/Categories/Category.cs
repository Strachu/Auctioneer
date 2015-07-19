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
		public int Id { get; set; }
		public string Name { get; set; }

		public virtual Category Parent { get; set; }
		public virtual ICollection<Category> SubCategories { get; set; }
	}
}
