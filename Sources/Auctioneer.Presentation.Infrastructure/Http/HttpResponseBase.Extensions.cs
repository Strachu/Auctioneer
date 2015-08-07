using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Infrastructure.Http
{
	public static class HttpResponseBaseExtensions
	{
		public static void SaveToCookieIfNotNull<T>(this HttpResponseBase response,
		                                            string cookieKey,
		                                            T value,
		                                            bool isPersistent = true)
		{
			Contract.Requires(response != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(cookieKey));

			if(value == null)
				return;

			var newCookie = new HttpCookie(cookieKey, value.ToString());

			if(isPersistent)
			{
				newCookie.Expires = DateTime.Now.AddYears(1);
			}

			response.SetCookie(newCookie);
		}
	}
}