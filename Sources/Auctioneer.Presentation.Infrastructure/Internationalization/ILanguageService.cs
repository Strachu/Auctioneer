using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Presentation.Infrastructure.Internationalization
{
	[ContractClass(typeof(ILanguageServiceContractClass))]
	public interface ILanguageService
	{
		bool IsSupportedLanguage(string languageCode);

		string FallbackLanguageCode
		{
			get;
		}
	}
}
