using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Auctioneer.Logic.BackgroundTasks;
using Auctioneer.Presentation.Infrastructure.ModelBinders;
using Auctioneer.Presentation.Infrastructure.Security;

using Owin;

using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Mvc;

namespace Auctioneer.Presentation
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			InitializeProfiling();

			ContainerConfig.RegisterTypes();

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			ModelBinderConfig.RegisterModelBinders();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		private void InitializeProfiling()
		{
			MiniProfilerEF6.Initialize();

			GlobalFilters.Filters.Add(new ProfilingActionFilter());

			var engines = ViewEngines.Engines.Select(x => new ProfilingViewEngine(x)).ToList();
			ViewEngines.Engines.Clear();
			foreach(var item in engines)
			{
				ViewEngines.Engines.Add(item);
			}
		}

		protected void Application_BeginRequest()
		{
			if (Request.IsLocal)
			{
				MiniProfiler.Start();
			} 
		}

		protected void Application_EndRequest()
		{
			MiniProfiler.Stop();
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			AuthenticationConfig.Configure(app);

			var backgroundTaskExecutor = new BackgroundTaskExecutor(DependencyResolver.Current.GetServices<IBackgroundTask>());
			backgroundTaskExecutor.StartAllTasks();
		}
	}
}
