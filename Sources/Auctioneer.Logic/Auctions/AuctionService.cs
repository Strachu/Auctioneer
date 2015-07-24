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

		public Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId,
		                                                             AuctionSortOrder sortBy,
		                                                             int pageIndex,
		                                                             int auctionsPerPage)
		{
			var auctions = from auction      in mContext.Auctions
			               from rootCategory in mContext.Categories
			               where rootCategory.Id == categoryId

			               join category in mContext.Categories
			               on auction.CategoryId equals category.Id
			               where auction.EndDate > DateTime.Now

			               where category.Left >= rootCategory.Left && category.Right <= rootCategory.Right
			               select auction;

			switch(sortBy)
			{
				case AuctionSortOrder.TitleAscending:
					auctions = auctions.OrderBy(x => x.Title);
					break;
				case AuctionSortOrder.TitleDescending:
					auctions = auctions.OrderByDescending(x => x.Title);
					break;

				default: // AuctionSortOrder.EndDateAscending
					auctions = auctions.OrderBy(x => x.EndDate);
					break;
				case AuctionSortOrder.EndDateDescending:
					auctions = auctions.OrderByDescending(x => x.EndDate);
					break;

				case AuctionSortOrder.PriceAscending:
					auctions = auctions.OrderBy(x => x.Price);
					break;
				case AuctionSortOrder.PriceDescending:
					auctions = auctions.OrderByDescending(x => x.Price);
					break;
			}

			return Task.FromResult(auctions.ToPagedList(pageIndex, auctionsPerPage));
		}

		public async Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults)
		{
			return await mContext.Auctions.OrderByDescending(x => x.CreationDate)
			                              .Take(maxResults)
			                              .ToListAsync();
		}
	}
}
