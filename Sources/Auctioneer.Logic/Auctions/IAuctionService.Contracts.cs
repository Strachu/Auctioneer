using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	[ContractClassFor(typeof(IAuctionService))]
	internal abstract class IAuctionServiceContractClass : IAuctionService
	{
		Task<IEnumerable<Auction>> IAuctionService.GetActiveAuctionsInCategory(int categoryId)
		{
			throw new NotImplementedException();
		}

		Task<IEnumerable<Auction>> IAuctionService.GetRecentAuctions(int maxResults)
		{
			Contract.Requires(maxResults > 0);

			throw new NotImplementedException();
		}
	}
}
