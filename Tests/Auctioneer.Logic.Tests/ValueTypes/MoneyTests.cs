using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Auctioneer.Logic.Currencies;
using Auctioneer.Logic.ValueTypes;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.ValueTypes
{
	public class MoneyTests
	{
		[SetUp]
		public void SetUp()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
		}

		[Test]
		public void DollarsPrintingFormatIsCorrect()
		{
			var money = new Money(10.50m, new Currency("$", CurrencySymbolPosition.BeforeAmount));

			Assert.That(money.ToString(), Is.EqualTo("$10.50"));
		}

		[Test]
		public void PolishZlotyPrintingFormatIsCorrect()
		{
			var money = new Money(1.50m, new Currency("zł", CurrencySymbolPosition.AfterAmountWithSpace));

			Assert.That(money.ToString(), Is.EqualTo("1.50 zł"));
		}

		[Test]
		public void EuroAfterAmountWithoutSpaceFormatIsCorrect()
		{
			var money = new Money(150m, new Currency("€", CurrencySymbolPosition.AfterAmount));

			Assert.That(money.ToString(), Is.EqualTo("150.00€"));
		}
	}
}
