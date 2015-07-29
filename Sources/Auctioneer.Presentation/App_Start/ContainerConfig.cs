using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

using Auctioneer.Logic;
using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Helpers;

using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;

namespace Auctioneer.Presentation
{
	public class ContainerConfig
	{
		public static void RegisterTypes()
		{
			var builder = new ContainerBuilder();

			builder.RegisterControllers(typeof(ContainerConfig).Assembly);

			builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
			builder.RegisterModelBinderProvider();
			builder.RegisterModule<AutofacWebTypesModule>();
			builder.RegisterSource(new ViewRegistrationSource());
			builder.RegisterFilterProvider();

			RegisterServices(builder);

			builder.RegisterType<AuctioneerDbContext>().As<AuctioneerDbContext>().As<IUnitOfWork>().InstancePerRequest();

			builder.RegisterType<BreadcrumbBuilder>().As<IBreadcrumbBuilder>().InstancePerDependency();
			builder.RegisterType<AuctionService>().As<IAuctionService>().InstancePerRequest().WithParameters(new Parameter[]
			{
				new NamedParameter("photoDirectoryPath",     HostingEnvironment.MapPath("~/Content/UserContent/Auctions/Photos")),
				new NamedParameter("thumbnailDirectoryPath", HostingEnvironment.MapPath("~/Content/UserContent/Auctions/Thumbnails"))
			});

			var container = builder.Build();	

			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}

		private static void RegisterServices(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(typeof(Auctioneer.Logic.AuctioneerDbContext).Assembly)
			       .Where(t => t.Name.EndsWith("Service"))
			       .AsImplementedInterfaces()
			       .InstancePerRequest();
		}
	}
}