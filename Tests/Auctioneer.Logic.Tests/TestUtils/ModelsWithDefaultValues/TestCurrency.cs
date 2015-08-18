using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Currencies;

namespace Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues
{
	internal class TestCurrency : Currency
	{
		private static int nextId = 1;

		public TestCurrency() : base(nextId.ToString(), CurrencySymbolPosition.AfterAmountWithSpace)
		{
			nextId++;
		}
	}
}
