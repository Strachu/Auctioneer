using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	[ContractClass(typeof(IAuctionServiceContractClass))]
	public interface IAuctionService
	{
		Task<IEnumerable<Auction>> GetActiveAuctionsInCategory(int categoryId);
		Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults);
	}
}
