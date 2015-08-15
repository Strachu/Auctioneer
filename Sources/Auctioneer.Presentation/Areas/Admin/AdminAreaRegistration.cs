using System.Web.Mvc;

using Auctioneer.Presentation.Areas.Admin.Controllers;
using Auctioneer.Presentation.Infrastructure.Internationalization;

namespace Auctioneer.Presentation.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration 
	{
		public override string AreaName 
		{
			get { return "Admin"; }
		}

		public override void RegisterArea(AreaRegistrationContext context) 
		{
			context.MapRoute(name: "Admin_defaultWithLanguage", url: "Admin/{lang}/{controller}/{action}/{id}", defaults: new
			{
				controller = "Home",
				action     = "Index",
				id         = UrlParameter.Optional
			},
			constraints: new
			{
				lang = new LanguageConstraint()
			},
			namespaces: new string[]
			{
				typeof(HomeController).Namespace
			});

			context.MapRoute(name: "Admin_default", url: "Admin/{controller}/{action}/{id}", defaults: new
			{
				controller = "Home",
				action     = "Index",
				id         = UrlParameter.Optional
			},
			namespaces: new string[]
			{
				typeof(HomeController).Namespace
			});
		}
	}
}