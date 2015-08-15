using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Auctioneer.Logic.Currencies;

namespace Auctioneer.Logic.ValueTypes
{
	public class Money
	{
		public Money(decimal amount, Currency currency)
		{
			Contract.Requires(amount > 0);
			Contract.Requires(currency != null);

			Amount   = amount;
			Currency = currency;
		}

		public decimal Amount
		{
			get;
			private set;
		}

		public virtual Currency Currency
		{
			get;
			private set;
		}

		public override string ToString()
		{
			var cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();

			cultureInfo.NumberFormat.CurrencySymbol = Currency.Symbol;

			switch(Currency.SymbolPosition)
			{
				case CurrencySymbolPosition.BeforeAmount:
					cultureInfo.NumberFormat.CurrencyPositivePattern = 0;
					break;

				case CurrencySymbolPosition.BeforeAmountWithSpace:
					cultureInfo.NumberFormat.CurrencyPositivePattern = 2;
					break;

				case CurrencySymbolPosition.AfterAmount:
					cultureInfo.NumberFormat.CurrencyPositivePattern = 1;
					break;

				case CurrencySymbolPosition.AfterAmountWithSpace:
					cultureInfo.NumberFormat.CurrencyPositivePattern = 3;
					break;
			}

			return Amount.ToString("C", cultureInfo);
		}

		// Required by Entity Framework
		protected Money()
		{
			
		}
		internal int Id { get; private set; }
	}
}
