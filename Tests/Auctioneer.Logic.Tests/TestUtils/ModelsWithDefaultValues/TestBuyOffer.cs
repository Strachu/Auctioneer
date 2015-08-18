using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;

namespace Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues
{
	internal class TestBuyOffer : BuyOffer
	{
		private static int nextId = 1;

		public TestBuyOffer()
		{
			base.Id        = nextId;
			base.UserId    = "1";
			base.AuctionId = 1;
			base.Amount    = 1.0m;
			base.Date      = DateTime.Now;

			nextId++;
		}
	}
}
