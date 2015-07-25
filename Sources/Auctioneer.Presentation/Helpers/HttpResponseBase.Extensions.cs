using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Helpers
{
	internal static class HttpResponseBaseExtensions
	{
		public static void SaveToCookieIfNotNull<T>(this HttpResponseBase response, string cookieKey, T value)
		{
			Contract.Requires(response != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(cookieKey));

			if(value == null)
				return;

			response.SetCookie(new HttpCookie(cookieKey, value.ToString()));
		}
	}
}