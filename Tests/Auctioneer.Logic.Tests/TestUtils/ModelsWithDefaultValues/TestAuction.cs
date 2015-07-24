using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;

namespace Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues
{
	internal class TestAuction : Auction
	{
		private static int nextId = 1;

		public TestAuction()
		{
			Id           = nextId++;
			Title        = "Not important";
			Description  = "Not important";

			CreationDate = DateTime.Now;
			EndDate      = DateTime.Now;		
		}
	}
}
