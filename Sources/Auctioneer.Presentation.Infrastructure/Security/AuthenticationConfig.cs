using System;

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

using Owin;

namespace Auctioneer.Presentation.Infrastructure.Security
{
	public class AuthenticationConfig
	{
		public static void Configure(IAppBuilder app)
		{
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationMode = AuthenticationMode.Active,
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				CookieSecure       = CookieSecureOption.Always,
				LoginPath          = new PathString("/User/Login"),
				ReturnUrlParameter = "returnUrl",
				ExpireTimeSpan     = TimeSpan.FromDays(30),
				SlidingExpiration  = true,
			});            
		}
	}
}
