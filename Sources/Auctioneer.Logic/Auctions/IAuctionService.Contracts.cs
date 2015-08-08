using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public Task<IPagedList<Auction>> GetAuctionsByUser(string userId,
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

		public Task RemoveAuctions(string removingUserId, params int[] ids)
		{
			Contract.Requires(removingUserId != null);
			Contract.Requires(ids != null);

			throw new NotImplementedException();
		}

		public bool CanBeBought(Auction auction, string buyerId)
		{
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		public Task Buy(int auctionId, string buyerId)
		{
			Contract.Requires(auctionId > 0);
			Contract.Requires(buyerId != null);

			throw new NotImplementedException();
		}
	}
}
