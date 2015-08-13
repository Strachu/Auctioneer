using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	public static class UrlHelperExtensionsWithOptionalParameters
	{
		public static string Action(this UrlHelper helper,
		                            string actionName = null,
		                            string controllerName = null,
		                            object routeValues = null,
		                            string protocol = null,
		                            string hostName = null)
		{
			return helper.Action(actionName: actionName, controllerName: controllerName, routeValues: routeValues,
			                     protocol: protocol, hostName: hostName);
		}
	}
}