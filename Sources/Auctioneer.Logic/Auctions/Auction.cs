﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Categories;

namespace Auctioneer.Logic.Auctions
{
	public class Auction
	{
		public int Id { get; set; }

		public string Title { get; set; }
		public string Description { get; set; }

		public DateTime CreationDate { get; set; }
		public DateTime EndDate { get; set; }

		public decimal Price { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }
	}
}