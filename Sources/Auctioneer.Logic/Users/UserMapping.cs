using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Users
{
	internal class UserMapping : EntityTypeConfiguration<User>
	{
		public UserMapping()
		{
			base.Property(x => x.Address).IsRequired();
			base.Property(x => x.FirstName).IsRequired();
			base.Property(x => x.LastName).IsRequired();

			base.Property(x => x.LockoutReason).IsOptional();
		}
	}
}
