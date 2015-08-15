using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Presentation.Infrastructure.Internationalization
{
	[ContractClassFor(typeof(ILanguageService))]
	internal abstract class ILanguageServiceContractClass : ILanguageService
	{
		IEnumerable<Language> ILanguageService.GetAllLanguages()
		{
			throw new NotImplementedException();
		}

		bool ILanguageService.IsSupportedLanguage(string languageCode)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(languageCode));

			throw new NotImplementedException();
		}

		string ILanguageService.FallbackLanguageCode
		{
			get
			{
				Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<string>()));

				throw new NotImplementedException();
			}
		}
	}
}
