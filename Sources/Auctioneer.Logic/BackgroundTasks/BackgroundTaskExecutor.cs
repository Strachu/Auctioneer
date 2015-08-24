using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Auctioneer.Logic.BackgroundTasks
{
	public class BackgroundTaskExecutor
	{
		private readonly IList<IBackgroundTask> mTasks;
		private readonly IList<Timer>           mTimers = new List<Timer>();

		public BackgroundTaskExecutor(IEnumerable<IBackgroundTask> tasks)
		{
			Contract.Requires(tasks != null);

			mTasks = tasks.ToList();
		}

		public void StartAllTasks()
		{
			for(int i = 0; i < mTasks.Count ;++i)
			{
				var task  = mTasks[i];
				var timer = new Timer
				{
					Interval  = task.TimeBetweenExecutions.TotalMilliseconds,
					Enabled   = true,
					AutoReset = false
				};

				timer.Elapsed += (sender, e) =>
				{
					task.Run();

					timer.Start();
				};

				timer.Start();

				mTimers.Add(timer);
			}
		}
	}
}
