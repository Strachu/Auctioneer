using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.AspNet.Identity;

using Auctioneer.Logic.Users;

namespace Auctioneer.Presentation.Infrastructure.Security
{
	public class BannedUserCheckAttribute : FilterAttribute, IAuthorizationFilter
	{
		private readonly string mControllerToRedirectToWhenBanned;
		private readonly string mActionToRedirectToWhenBanned;

		public BannedUserCheckAttribute(string bannedUserRedirectControllerName, string bannedUserRedirectActionName)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(bannedUserRedirectControllerName));
			Contract.Requires(!String.IsNullOrWhiteSpace(bannedUserRedirectActionName));

			mControllerToRedirectToWhenBanned = bannedUserRedirectControllerName;
			mActionToRedirectToWhenBanned     = bannedUserRedirectActionName;
		}

		public void OnAuthorization(AuthorizationContext filterContext)
		{
			if(!filterContext.HttpContext.User.Identity.IsAuthenticated)
				return;

			var authManager = DependencyResolver.Current.GetService<IAuthenticationManager>();
			var userService = DependencyResolver.Current.GetService<IUserService>();

			var user = userService.GetUserById(filterContext.HttpContext.User.Identity.GetUserId()).Result;
			if(!user.IsBanned)
				return;

			filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
			{
				{ "controller", mControllerToRedirectToWhenBanned },
				{ "action",     mActionToRedirectToWhenBanned },
				{ "id",         user.Id }
			});

			authManager.SignOut();
		}
	}
}