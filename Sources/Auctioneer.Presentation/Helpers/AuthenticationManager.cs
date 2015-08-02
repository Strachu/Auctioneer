using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Auctioneer.Presentation.Helpers
{
	public class AuthenticationManager : SignInManager<User, string>
	{
		public AuthenticationManager(UserService userService, IAuthenticationManager authenticationManager) :
			base(userService, authenticationManager)
		{
		}

		public void SignOut()
		{
			base.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
		}
	}
}