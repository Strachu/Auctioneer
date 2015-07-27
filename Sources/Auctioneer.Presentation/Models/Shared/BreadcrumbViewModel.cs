using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Models
{
	public class BreadcrumbViewModel
	{
		public IEnumerable<Item> Items;

		public class Item
		{
			public string Name;

			[DataType(DataType.Url)]
			public string TargetUrl;
		}
	}
}