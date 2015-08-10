using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Http;

namespace Auctioneer.Presentation.Infrastructure.Internationalization
{
	public class LanguageFilterAttribute : ActionFilterAttribute
	{
		private const string LANG_PARAMETER_NAME = "lang";

		public ILanguageService LanguageService { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var language = ReadRequestedLanguageCode(filterContext);

			if(!LanguageService.IsSupportedLanguage(language))
			{
				language = LanguageService.FallbackLanguageCode;
			}

			ChangeUrlToIncludeLanguageCode(filterContext, language);

			filterContext.HttpContext.Response.SaveToCookieIfNotNull(LANG_PARAMETER_NAME, language, isPersistent: true);

			SetCurrentLanguage(language);
		}

		private string ReadRequestedLanguageCode(ActionExecutingContext filterContext)
		{
			var langFromUrl = filterContext.RouteData.Values[LANG_PARAMETER_NAME];
			if(langFromUrl != null)
				return langFromUrl.ToString();

			var langFromCookie = filterContext.HttpContext.Request.ReadStringFromCookie(LANG_PARAMETER_NAME);
			if(langFromCookie != null)
				return langFromCookie;

			var langFromHeader = GetFirstSupportedLanguageCodeFromHeader(filterContext.HttpContext.Request);
			if(langFromHeader != null)
				return langFromHeader;

			return LanguageService.FallbackLanguageCode;
		}

		private string GetFirstSupportedLanguageCodeFromHeader(HttpRequestBase request)
		{
			if(request.UserLanguages == null)
				return null;

			return request.UserLanguages.FirstOrDefault(x =>
			{
				var twoLetterLanguageCode = x.Split(';').First().Split('-').First();

				return LanguageService.IsSupportedLanguage(twoLetterLanguageCode);
			});
		}

		private void ChangeUrlToIncludeLanguageCode(ActionExecutingContext filterContext, string languageCode)
		{
			if(filterContext.RouteData.Values[LANG_PARAMETER_NAME] != null)
				return;

			var newRoute = filterContext.RouteData.Values;

			newRoute[LANG_PARAMETER_NAME] = languageCode;

			filterContext.Result = new RedirectToRouteResult(newRoute);
		}

		private void SetCurrentLanguage(string languageCode)
		{
			var newCulture = new CultureInfo(languageCode);

			// TODO add multiple currency support
			newCulture.NumberFormat.CurrencySymbol = "zł";

			Thread.CurrentThread.CurrentCulture   = new CultureInfo(languageCode);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageCode);
		}
	}
}
