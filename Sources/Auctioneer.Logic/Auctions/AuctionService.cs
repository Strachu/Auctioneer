using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PagedList;

namespace Auctioneer.Logic.Auctions
{
	public class AuctionService : IAuctionService
	{
		private readonly AuctioneerDbContext mContext;

		public AuctionService(AuctioneerDbContext context)
		{
			mContext = context;
		}

		public Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId, int pageIndex, int auctionsPerPage)
		{
			var auctions = from auction      in mContext.Auctions
			               from rootCategory in mContext.Categories
			               where rootCategory.Id == categoryId

			               join category in mContext.Categories
			               on auction.CategoryId equals category.Id

			               where category.Left >= rootCategory.Left && category.Right <= rootCategory.Right
			               select auction;

			return Task.FromResult(auctions.Where(x => x.EndDate > DateTime.Now).OrderBy(x => x.Id).ToPagedList(pageIndex, auctionsPerPage));
		}

		public async Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults)
		{
			return await mContext.Auctions.OrderByDescending(x => x.CreationDate)
			                              .Take(maxResults)
			                              .ToListAsync();
		}
	}
}
