using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Validation;

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
		                                            TimeSpan createdIn,
		                                            string titleFilter = null,
		                                            AuctionStatusFilter statusFilter = AuctionStatusFilter.All,
		                                            int pageIndex = 1,
		                                            int auctionsPerPage = int.MaxValue);

		Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults);

		Task<Auction> GetById(int id);

		Task AddAuction(Auction newAuction, IEnumerable<Stream> photosData);

		bool CanBeRemoved(Auction auction, string userId);
		Task RemoveAuctions(IReadOnlyCollection<int> ids, string removingUserId, IValidationErrorNotifier errors);

		bool CanBeBought(Auction auction, string buyerId);
		Task Buy(int auctionId, string buyerId, IValidationErrorNotifier errors);
	}
}
