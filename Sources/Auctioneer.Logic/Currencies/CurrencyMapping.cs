using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Currencies
{
	internal class CurrencyMapping : EntityTypeConfiguration<Currency>
	{
		public CurrencyMapping()
		{
			base.HasKey(x => x.Symbol);
			base.Property(x => x.SymbolPosition).IsRequired();
		}
	}
}
