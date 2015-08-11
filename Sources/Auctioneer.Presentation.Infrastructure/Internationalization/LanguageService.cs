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
		private readonly List<Language> mCachedLanguageList = new List<Language>();

		public IEnumerable<Language> GetAllLanguages()
		{
			if(!mCachedLanguageList.Any())
			{
				// Based on http://stackoverflow.com/a/11047894/2579010
				var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				var resourceManager = new ResourceManager(typeof(Auctioneer.Resources.Shared.Layout));

				foreach(var culture in allCultures)
				{
					var resourcesForThisCulture = resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
					if(resourcesForThisCulture != null)
					{
						var language = new Language
						{
							LangCode    = !culture.Equals(CultureInfo.InvariantCulture) ? culture.TwoLetterISOLanguageName : "en",
							DisplayName = !culture.Equals(CultureInfo.InvariantCulture) ? culture.DisplayName              : "English"
						};

						mCachedLanguageList.Add(language);
					}
				}
			}

			return mCachedLanguageList;
		}

		public bool IsSupportedLanguage(string languageCode)
		{
			return GetAllLanguages().Select(x => x.LangCode).Contains(languageCode);
		}

		public string FallbackLanguageCode
		{
			get { return "en"; }
		}
	}
}
