using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Currencies
{
	[ContractClassFor(typeof(ICurrencyService))]
	internal abstract class ICurrencyServiceContractClass : ICurrencyService
	{
		Task<IEnumerable<Currency>> ICurrencyService.GetAllCurrencies()
		{
			throw new NotImplementedException();
		}

		Task<Currency> ICurrencyService.GetCurrencyBySymbol(string symbol)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(symbol));

			throw new NotImplementedException();
		}
	}
}
