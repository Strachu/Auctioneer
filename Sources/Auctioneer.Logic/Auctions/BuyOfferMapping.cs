using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	internal class BuyOfferMapping : EntityTypeConfiguration<BuyOffer>
	{
		public BuyOfferMapping()
		{
			base.HasKey(x => x.Id);
			base.Property(x => x.AuctionId).IsRequired();

			base.Property(x => x.Date).IsRequired();
			base.Property(x => x.Amount).HasPrecision(9, 2);

			base.HasRequired(x => x.User).WithMany().HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
		}
	}
}
