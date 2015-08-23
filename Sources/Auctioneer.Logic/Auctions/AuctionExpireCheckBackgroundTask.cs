using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.BackgroundTasks;
using Auctioneer.Logic.Users;

namespace Auctioneer.Logic.Auctions
{
	public class AuctionExpireCheckBackgroundTask : IBackgroundTask
	{
		private readonly AuctioneerDbContext mContext;
		private readonly IUserNotifier       mUserNotifier;

		public AuctionExpireCheckBackgroundTask(AuctioneerDbContext context, IUserNotifier userNotifier)
		{
			Contract.Requires(context != null);
			Contract.Requires(userNotifier != null);

			mContext      = context;
			mUserNotifier = userNotifier;
		}

		public void Run()
		{
			var lastRunTime               = mContext.BackgroundTasksData.AuctionExpiryCheckLastRun;
			var currentRunTime            = DateTime.Now;
			var auctionsEndedSinceLastRun = mContext.Auctions.Include(x => x.Seller)
			                                                 .Include(x => x.BuyoutPrice.Currency)
			                                                 .Include(x => x.MinimumPrice.Currency)
			                                                 .Include(x => x.Offers)
			                                                 .Where(x => x.EndDate > lastRunTime && x.EndDate <= currentRunTime)
			                                                 .Where(x => x.Offers.All(offer => offer.Amount < x.BuyoutPrice.Amount));

			foreach(var auction in auctionsEndedSinceLastRun)
			{
				switch(auction.Status)
				{
					case AuctionStatus.Sold:
						mUserNotifier.NotifyAuctionWon(auction.Buyer, auction);
						mUserNotifier.NotifyAuctionSold(auction.Seller, auction);
						break;

					case AuctionStatus.Expired:
						mUserNotifier.NotifyAuctionExpired(auction.Seller, auction);
						break;
				}
			}

			mContext.BackgroundTasksData.AuctionExpiryCheckLastRun = currentRunTime;
			mContext.SaveChanges();
		}

		public TimeSpan TimeBetweenExecutions
		{
			get { return TimeSpan.FromMinutes(1); }
		}
	}
}
