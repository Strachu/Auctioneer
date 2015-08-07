using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Auctioneer.Presentation.Infrastructure.ModelBinders;
using Auctioneer.Presentation.Infrastructure.Security;

using Owin;

namespace Auctioneer.Presentation
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			ModelBinderConfig.RegisterModelBinders();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			ContainerConfig.RegisterTypes();
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			AuthenticationConfig.Configure(app);
		}
	}
}
