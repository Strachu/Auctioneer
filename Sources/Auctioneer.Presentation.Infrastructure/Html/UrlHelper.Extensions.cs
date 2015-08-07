using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	public static class UrlHelperExtensions
	{
		public static string ActionWithCurrentParameters(this UrlHelper helper,
		                                                 string actionName,
		                                                 string controllerName = null,
		                                                 object routeValues = null)
		{
			var currentParameters = HttpContext.Current.Request.QueryString;
			var routeDictionary   = new RouteValueDictionary();
			var routeOverrides    = new RouteValueDictionary(routeValues);

			foreach(var parameter in currentParameters.AllKeys)
			{
				routeDictionary[parameter] = currentParameters[parameter];
			}

			foreach(var routeOverride in routeOverrides)
			{
				routeDictionary[routeOverride.Key] = routeOverride.Value;
			}

			return helper.Action(actionName, controllerName, routeDictionary);
		}
	}
}