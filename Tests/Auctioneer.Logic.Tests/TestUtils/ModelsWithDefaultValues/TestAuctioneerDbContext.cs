using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues
{
	internal class TestAuctioneerDbContext : AuctioneerDbContext
	{
		public TestAuctioneerDbContext(DbConnection connection) : base(connection)
		{
		}

		protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// A bit hacky way to provide default values for properties not needed for tests but still satisfy constraints.
			modelBuilder.RegisterEntityType(typeof(TestAuction));
			modelBuilder.RegisterEntityType(typeof(TestCategory));
			modelBuilder.RegisterEntityType(typeof(TestUser));

			modelBuilder.Entity<TestAuction>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			modelBuilder.Entity<TestCategory>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			modelBuilder.Entity<TestUser>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
		}
	}
}
