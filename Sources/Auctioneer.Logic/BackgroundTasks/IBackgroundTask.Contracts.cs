using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.BackgroundTasks
{
	[ContractClassFor(typeof(IBackgroundTask))]
	internal abstract class IBackgroundTaskContractClass : IBackgroundTask
	{
		TimeSpan IBackgroundTask.TimeBetweenExecutions
		{
			get
			{
				Contract.Ensures(Contract.Result<TimeSpan>().TotalMilliseconds >= 1);

				throw new NotImplementedException();
			}
		}

		void IBackgroundTask.Run()
		{
			throw new NotImplementedException();
		}
	}
}
