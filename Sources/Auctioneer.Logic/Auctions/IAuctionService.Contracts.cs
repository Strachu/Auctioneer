using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Validation;

using PagedList;

namespace Auctioneer.Logic.Auctions
{
	[ContractClassFor(typeof(IAuctionService))]
	internal abstract class IAuctionServiceContractClass : IAuctionService
	{
		Task<IPagedList<Auction>> IAuctionService.GetActiveAuctionsInCategory(int categoryId,
		                                                                      AuctionSortOrder sortBy,
		                                                                      int pageIndex,
		                                                                      int auctionsPerPage)
		{
			Contract.Requires(pageIndex >= 1);
			Contract.Requires(auctionsPerPage >= 1);

			throw new NotImplementedException();
		}

		Task<IPagedList<Auction>> IAuctionService.GetAuctionsByUser(string userId,
		                                                            TimeSpan createdIn,
		                                                            string titleFilter,
		                                                            AuctionStatusFilter statusFilter,
		                                                            int pageIndex,
		                                                            int auctionsPerPage)
		{
			Contract.Requires(pageIndex >= 1);
			Contract.Requires(auctionsPerPage >= 1);

			throw new NotImplementedException();
		}

		Task<IEnumerable<Auction>> IAuctionService.GetRecentAuctions(int maxResults)
		{
			Contract.Requires(maxResults > 0);

			throw new NotImplementedException();
		}

		Task<Auction> IAuctionService.GetById(int id)
		{
			throw new NotImplementedException();
		}

		Task IAuctionService.AddAuction(Auction newAuction, IEnumerable<System.IO.Stream> photosData)
		{
			Contract.Requires(newAuction != null);
			Contract.Requires(photosData != null);

			throw new NotImplementedException();
		}

		Task<bool> IAuctionService.CanBeRemoved(Auction auction, string userId)
		{
			Contract.Requires(auction != null);
			Contract.Requires(userId != null);

			throw new NotImplementedException();
		}
	
		Task IAuctionService.RemoveAuctions(IReadOnlyCollection<int> ids, string removingUserId, IValidationErrorNotifier errors)
		{
			Contract.Requires(removingUserId != null);
			Contract.Requires(ids != null);
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}

		bool IAuctionService.CanBeBought(Auction auction, string buyerId)
		{
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		Task IAuctionService.Buy(int auctionId, string buyerId, IValidationErrorNotifier errors)
		{
			Contract.Requires(auctionId > 0);
			Contract.Requires(buyerId != null);
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}

		Task<bool> IAuctionService.CanBeMoved(Auction auction, string userId)
		{
			Contract.Requires(auction != null);
			Contract.Requires(userId != null);

			throw new NotImplementedException();
		}

		Task IAuctionService.MoveAuction(int auctionId, int newCategoryId, string movingUserId, IValidationErrorNotifier errors)
		{
			Contract.Requires(auctionId > 0);
			Contract.Requires(newCategoryId > 0);
			Contract.Requires(movingUserId != null);
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}
	}
}
