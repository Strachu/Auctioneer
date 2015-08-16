using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Routing;

namespace Auctioneer.Presentation.Infrastructure.Http
{
	public static class RouteValueDictionaryExtensions
	{
		public static void Merge(this RouteValueDictionary first, RouteValueDictionary second)
		{
			Contract.Requires(first != null);
			Contract.Requires(second != null);

			foreach(var pair in second)
			{
				first[pair.Key] = pair.Value;
			}
		}
	}
}