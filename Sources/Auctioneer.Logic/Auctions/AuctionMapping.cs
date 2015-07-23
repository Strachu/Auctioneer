using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	internal class AuctionMapping : EntityTypeConfiguration<Auction>
	{
		public AuctionMapping()
		{
			base.HasKey(x => x.Id);
			base.Property(x => x.Title).IsRequired();
			base.Property(x => x.Description).IsRequired();

			base.Property(x => x.Price).HasPrecision(9, 2);
		}
	}
}
