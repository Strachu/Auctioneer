using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;

namespace Auctioneer.Logic
{
	public class AuctioneerDbContext : DbContext, IUnitOfWork
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Auction> Auctions { get; set; }

		public AuctioneerDbContext() : base("AuctioneerDbContext")
		{
#if DEBUG
			base.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
		}

		public AuctioneerDbContext(DbConnection connection) : base(connection, true)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Configurations.AddFromAssembly(GetType().Assembly);
		}

		async Task IUnitOfWork.Commit()
		{
			await SaveChangesAsync();
		}
	}
}
