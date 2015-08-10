using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

using Auctioneer.Presentation.Infrastructure.Internationalization;

namespace Auctioneer.Presentation
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			var constraintsResolver = new DefaultInlineConstraintResolver();
			constraintsResolver.ConstraintMap.Add("language", typeof(LanguageConstraint));

			routes.MapMvcAttributeRoutes(constraintsResolver);

			// TODO Currently there is a requirements to specify every route twice - once with and once without language.
			// Can it be solved somehow?

			routes.MapRoute(name: "DefaultWithLanguage", url: "{lang}/{controller}/{action}/{id}", defaults: new
			{
				controller = "Home",
				action     = "Index",
				id         = UrlParameter.Optional
			},
			constraints: new
			{
				lang = new LanguageConstraint()
			});

			routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new
			{
				controller = "Home",
				action     = "Index",
				id         = UrlParameter.Optional
			});
		}
	}
}
