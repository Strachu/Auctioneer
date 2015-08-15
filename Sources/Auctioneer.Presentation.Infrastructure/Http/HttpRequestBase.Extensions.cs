using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Infrastructure.Http
{
	public static class HttpRequestBaseExtensions
	{
		public static T ReadFromCookie<T>(this HttpRequestBase request, string cookieKey, Func<string, T> valueParser)
		{
			Contract.Requires(request != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(cookieKey));
			Contract.Requires(valueParser != null);

			if(request.Cookies[cookieKey] == null)
				return default(T);

			return valueParser(request.Cookies[cookieKey].Value);
		}

		public static int? ReadIntFromCookie(this HttpRequestBase request, string cookieKey)
		{
			return request.ReadFromCookie<int?>(cookieKey, x => Int32.Parse(x));
		}

		public static string ReadStringFromCookie(this HttpRequestBase request, string cookieKey)
		{
			return request.ReadFromCookie(cookieKey, x => x);
		}

		public static TEnum? ReadEnumFromCookie<TEnum>(this HttpRequestBase request, string cookieKey) where TEnum : struct
		{
			return request.ReadFromCookie<TEnum?>(cookieKey, x => (TEnum)Enum.Parse(typeof(TEnum), x));
		}
	}
}