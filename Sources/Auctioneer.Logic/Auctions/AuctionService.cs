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
			return await mContext.Auctions.Where(x => x.CategoryId == categoryId)
			                              .Where(x => x.EndDate > DateTime.Now)
			                              .ToListAsync();
		}
	}
}
