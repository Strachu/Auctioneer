using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.BackgroundTasks
{
	public class BackgroundTasksDataMapping : EntityTypeConfiguration<BackgroundTasksData>
	{
		public BackgroundTasksDataMapping()
		{
			base.HasKey(x => x.Id);
		}
	}
}
