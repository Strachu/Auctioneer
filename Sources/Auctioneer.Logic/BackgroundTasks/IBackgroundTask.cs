using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.BackgroundTasks
{
	[ContractClass(typeof(IBackgroundTaskContractClass))]
	public interface IBackgroundTask
	{
		TimeSpan TimeBetweenExecutions
		{
			get;
		}

		void Run();
	}
}
