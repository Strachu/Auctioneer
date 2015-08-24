using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.BackgroundTasks
{
	public class BackgroundTasksData
	{
		public DateTime AuctionExpiryCheckLastRun { get; set; }

		internal int Id { get; private set; }
	}
}
