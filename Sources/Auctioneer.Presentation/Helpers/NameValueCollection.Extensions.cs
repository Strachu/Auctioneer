using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Auctioneer.Presentation.Helpers
{
	public static class NameValueCollectionExtensions
	{
		public static RouteValueDictionary ToRouteDictionary(this NameValueCollection collection)
		{
			var routeDictionary = new RouteValueDictionary();

			foreach(var parameter in collection.AllKeys)
			{
				routeDictionary[parameter] = collection[parameter];
			}

			return routeDictionary;
		}
	}
}