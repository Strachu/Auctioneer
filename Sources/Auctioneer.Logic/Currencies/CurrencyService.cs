using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Currencies
{
	public class CurrencyService : ICurrencyService
	{
		private readonly AuctioneerDbContext mContext;

		public CurrencyService(AuctioneerDbContext context)
		{
			Contract.Requires(context != null);

			mContext = context;
		}

		public async Task<IEnumerable<Currency>> GetAllCurrencies()
		{
			return await mContext.Currencies.ToListAsync().ConfigureAwait(false);
		}

		public async Task<Currency> GetCurrencyBySymbol(string symbol)
		{
			return await mContext.Currencies.FindAsync(symbol).ConfigureAwait(false);
		}
	}
}
