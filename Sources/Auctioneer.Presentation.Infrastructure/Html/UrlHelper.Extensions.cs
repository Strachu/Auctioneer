using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Auctioneer.Presentation.Infrastructure.Http;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	public static class UrlHelperExtensions
	{
		public static string ActionWithCurrentParameters(this UrlHelper helper,
		                                                 string actionName = null,
		                                                 string controllerName = null,
		                                                 object routeValues = null)
		{
			var routeDictionary = new RouteValueDictionary(routeValues);

			return helper.ActionWithCurrentParameters(controllerName: controllerName, actionName: actionName,
			                                          routeOverrides: routeDictionary);
		}

		public static string ActionWithCurrentParameters(this UrlHelper helper,
		                                                 RouteValueDictionary routeOverrides,
		                                                 string actionName = null,
		                                                 string controllerName = null)
		{
			var routeDictionary = HttpContext.Current.Request.QueryString.ToRouteDictionary();

			foreach(var routeOverride in routeOverrides)
			{
				routeDictionary[routeOverride.Key] = routeOverride.Value;
			}

			return helper.Action(actionName, controllerName, routeDictionary);
		}
	}
}