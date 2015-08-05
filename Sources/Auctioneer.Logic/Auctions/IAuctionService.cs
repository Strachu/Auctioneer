using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
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

		Task<IPagedList<Auction>> GetAuctionsByUser(string userId,
		                                            int pageIndex,
		                                            int auctionsPerPage);

		Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults);

		Task<Auction> GetById(int id);

		Task AddAuction(Auction newAuction);
		Task StoreAuctionPhotos(int auctionId, IEnumerable<Stream> dataStreams);
	}
}
