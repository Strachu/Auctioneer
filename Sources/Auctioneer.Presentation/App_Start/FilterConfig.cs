using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Filters;

namespace Auctioneer.Presentation
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new RequireHttpsAttribute());
			filters.Add(new ObjectNotFoundExceptionHandlerAttribute());
			filters.Add(new HandleErrorAttribute());
		}
	}
}
