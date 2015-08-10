using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Presentation.Infrastructure.Internationalization
{
	public class LanguageService : ILanguageService
	{
		private readonly List<string> mSupportedLanguages = new List<string>();

		public LanguageService()
		{
			RetrieveSupportedLanguages();
		}

		private void RetrieveSupportedLanguages()
		{
			// Based on http://stackoverflow.com/a/11047894/2579010
			var allCultures     = CultureInfo.GetCultures(CultureTypes.AllCultures);
			var resourceManager = new ResourceManager(typeof(Auctioneer.Resources.Shared.Layout));

			foreach(var culture in allCultures)
			{
				var resourcesForThisCulture = resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
				if(resourcesForThisCulture != null)
				{
					var code = !culture.Equals(CultureInfo.InvariantCulture) ? culture.TwoLetterISOLanguageName : "en";

					mSupportedLanguages.Add(code);
				}
			}
		}

		public bool IsSupportedLanguage(string languageCode)
		{
			return mSupportedLanguages.Contains(languageCode);
		}

		public string FallbackLanguageCode
		{
			get { return "en"; }
		}
	}
}
