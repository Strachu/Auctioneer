using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	public class AuctionService : IAuctionService
	{
		private readonly AuctioneerDbContext mContext;

		public AuctionService(AuctioneerDbContext context)
		{
			mContext = context;
		}

		public async Task<IEnumerable<Auction>> GetActiveAuctionsInCategory(int categoryId)
		{
			var rootCategory = await mContext.Categories.FindAsync(categoryId);

			var auctions = from auction  in mContext.Auctions
			               join category in mContext.Categories
			               on auction.CategoryId equals category.Id
			               where category.Left >= rootCategory.Left && category.Right <= rootCategory.Right
			               select auction;

			return await auctions.Where(x => x.EndDate > DateTime.Now).ToListAsync();
		}

		public async Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults)
		{
			return await mContext.Auctions.OrderByDescending(x => x.CreationDate)
			                              .Take(maxResults)
			                              .ToListAsync();
		}
	}
}
