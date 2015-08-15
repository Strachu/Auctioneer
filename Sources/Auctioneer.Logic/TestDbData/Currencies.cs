using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Currencies;

namespace Auctioneer.Logic.TestDbData
{
	internal static class Currencies
	{
		public static void Add(AuctioneerDbContext context)
		{
			var currencies = new Currency[]
			{
				new Currency("$",  CurrencySymbolPosition.BeforeAmount),
				new Currency("€",  CurrencySymbolPosition.AfterAmountWithSpace),
				new Currency("zł", CurrencySymbolPosition.AfterAmountWithSpace),
				new Currency("£",  CurrencySymbolPosition.BeforeAmount),
			};

			context.Currencies.AddRange(currencies);
			context.SaveChanges();
		}
	}
}
