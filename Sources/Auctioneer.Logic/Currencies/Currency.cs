using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Currencies
{
	public enum CurrencySymbolPosition
	{
		BeforeAmount,
		BeforeAmountWithSpace,

		AfterAmount,
		AfterAmountWithSpace
	}

	public class Currency
	{
		public Currency(string symbol, CurrencySymbolPosition symbolPosition)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(symbol));

			Symbol         = symbol;
			SymbolPosition = symbolPosition;
		}

		public string Symbol
		{
			get;
			private set;
		}

		public CurrencySymbolPosition SymbolPosition
		{
			get;
			private set;
		}

		// Required by Entity Framework
		protected Currency()
		{
			
		}
	}
}
