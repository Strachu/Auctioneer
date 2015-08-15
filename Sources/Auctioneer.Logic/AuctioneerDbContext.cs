using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Currencies;
using Auctioneer.Logic.Users;

using Microsoft.AspNet.Identity.EntityFramework;

namespace Auctioneer.Logic
{
	public class AuctioneerDbContext : IdentityDbContext<User>
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Auction> Auctions { get; set; }
		public DbSet<Currency> Currencies { get; set; }

		public AuctioneerDbContext() : base("AuctioneerDbContext")
		{
			Init();
		}

		public AuctioneerDbContext(DbConnection connection) : base(connection, true)
		{
			Init();
		}

		private void Init()
		{
#if DEBUG
			base.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
			base.RequireUniqueEmail = true;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Configurations.AddFromAssembly(typeof(AuctioneerDbContext).Assembly);
		}
	}
}
