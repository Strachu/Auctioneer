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
		Task<IPagedList<Auction>> GetAllActiveAuctions(string titleFilter = null,
		                                               AuctionSortOrder sortBy = AuctionSortOrder.TitleAscending,
		                                               int pageIndex = 1,
		                                               int auctionsPerPage = int.MaxValue);

		Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId,
		                                                      string titleFilter = null,
		                                                      AuctionSortOrder sortBy = AuctionSortOrder.TitleAscending,
		                                                      int pageIndex = 1,
		                                                      int auctionsPerPage = int.MaxValue);

		Task<IPagedList<Auction>> GetAuctionsByUser(string userId,
		                                            TimeSpan createdIn,
		                                            string titleFilter = null,
		                                            AuctionStatusFilter statusFilter = AuctionStatusFilter.All,
		                                            int pageIndex = 1,
		                                            int auctionsPerPage = int.MaxValue);

		Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults);

		Task<Auction> GetById(int id);

		Task AddAuction(Auction newAuction, IEnumerable<Stream> photosData);

		Task<bool> CanBeRemoved(Auction auction, string userId);
		Task RemoveAuctions(IReadOnlyCollection<int> ids, string removingUserId, IValidationErrorNotifier errors);

		bool CanBeBought(Auction auction, string buyerId);
		Task Bid(int auctionId, string buyerId, decimal bidAmount, IValidationErrorNotifier errors);
		Task Buyout(int auctionId, string buyerId, IValidationErrorNotifier errors);

		Task<bool> CanBeMoved(Auction auction, string userId);
		Task MoveAuction(int auctionId, int newCategoryId, string movingUserId, IValidationErrorNotifier errors);
	}
}
