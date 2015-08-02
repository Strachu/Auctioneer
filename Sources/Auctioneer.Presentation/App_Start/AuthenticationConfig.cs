using System;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

using Owin;

namespace Auctioneer.Presentation
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
				SlidingExpiration  = true,
			});            
		}
	}
}
