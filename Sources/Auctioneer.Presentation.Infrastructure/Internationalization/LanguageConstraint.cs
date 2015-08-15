using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Auctioneer.Presentation.Infrastructure.Internationalization
{
	public class LanguageConstraint : IRouteConstraint
	{
		private readonly ILanguageService mLanguageService;

		public LanguageConstraint()
		{
			mLanguageService = DependencyResolver.Current.GetService<ILanguageService>();
		}

		public bool Match(HttpContextBase httpContext,
		                  Route route,
		                  string parameterName,
		                  RouteValueDictionary values,
		                  RouteDirection routeDirection)
		{
			var lang = values[parameterName].ToString();

			return mLanguageService.IsSupportedLanguage(lang);
		}
	}
}
