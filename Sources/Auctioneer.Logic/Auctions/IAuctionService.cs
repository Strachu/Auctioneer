using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PagedList;

namespace Auctioneer.Logic.Auctions
{
	[ContractClass(typeof(IAuctionServiceContractClass))]
	public interface IAuctionService
	{
		Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId,
		                                                      AuctionSortOrder sortBy,
		                                                      int pageIndex,
		                                                      int auctionsPerPage);

		Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults);
	}
}
