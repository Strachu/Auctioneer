using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

using Auctioneer.Logic;
using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Emails;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Infrastructure.Security;

using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;

using Postal;

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

			builder.Register(x => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
			builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();

			RegisterServices(builder);

			builder.RegisterType<AuctioneerDbContext>().InstancePerRequest();

			builder.RegisterType<BreadcrumbBuilder>().As<IBreadcrumbBuilder>().InstancePerDependency();
			builder.RegisterType<AuthenticationManager>().As<IAuthenticationManager>().InstancePerRequest();
			builder.RegisterType<EmailUserNotifier>().As<IUserNotifier>().InstancePerLifetimeScope();
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
			builder.RegisterAssemblyTypes(typeof(IAuctionService).Assembly)
			       .Where(t => t.Name.EndsWith("Service"))
					 .AsSelf()
			       .AsImplementedInterfaces()
			       .InstancePerRequest();
		}
	}
}