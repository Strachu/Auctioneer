using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Auctioneer.Logic.Users;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Auctioneer.Presentation.Helpers
{
	public class AuthenticationManager : SignInManager<User, string>, IAuthenticationManager
	{
		public AuthenticationManager(UserService userService, Microsoft.Owin.Security.IAuthenticationManager authenticationManager) 
			: base(userService, authenticationManager)
		{
		}

		public Task<SignInStatus> SignIn(string userName, string password, bool rememberMe)
		{
			return base.PasswordSignInAsync(userName, password, rememberMe, shouldLockout: true);
		}

		public Task SignOut()
		{
			base.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return Task.FromResult(0);
		}
	}
}