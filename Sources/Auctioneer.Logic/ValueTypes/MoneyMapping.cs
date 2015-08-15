using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.ValueTypes
{
	internal class MoneyMapping : EntityTypeConfiguration<Money>
	{
		public MoneyMapping()
		{
			// Money is entity instead of complex type because Entity Framework doesn't allow navigation properties in
			// complex types.

			base.HasKey(x => x.Id);

			base.Property(x => x.Amount).HasPrecision(9, 2);

			base.HasRequired(x => x.Currency).WithMany();
		}
	}
}
