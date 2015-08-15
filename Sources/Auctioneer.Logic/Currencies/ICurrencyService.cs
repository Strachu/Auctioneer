using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Currencies
{
	[ContractClass(typeof(ICurrencyServiceContractClass))]
	public interface ICurrencyService
	{
		Task<IEnumerable<Currency>> GetAllCurrencies();
		Task<Currency> GetCurrencyBySymbol(string symbol);
	}
}
