using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.TestDbData
{
	internal class DbContextInitializer : DropCreateDatabaseIfModelChanges<AuctioneerDbContext>
	{
		protected override void Seed(AuctioneerDbContext context)
		{
			Users.Add(context);
			Categories.Add(context);
			Currencies.Add(context);
			Auctions.Add(context);

			base.Seed(context);
		}
	}
}
