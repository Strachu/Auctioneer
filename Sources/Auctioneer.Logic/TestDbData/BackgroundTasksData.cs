using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.TestDbData
{
	internal static class BackgroundTasksData
	{
		public static void Add(AuctioneerDbContext context)
		{
			context.Set<BackgroundTasks.BackgroundTasksData>().Add(new BackgroundTasks.BackgroundTasksData
			{
				AuctionExpiryCheckLastRun = DateTime.Now
			});

			context.SaveChanges();
		}
	}
}
