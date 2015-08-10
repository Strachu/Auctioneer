using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Filters;
using Auctioneer.Presentation.Infrastructure.Internationalization;

namespace Auctioneer.Presentation
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new RequireHttpsAttribute());
			filters.Add(new LanguageFilterAttribute { LanguageService = DependencyResolver.Current.GetService<ILanguageService>() });
			filters.Add(new ObjectNotFoundExceptionHandlerAttribute());
			filters.Add(new HandleErrorAttribute());
		}
	}
}
