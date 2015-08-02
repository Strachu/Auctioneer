using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Mappers;
using Auctioneer.Presentation.Models.User;

using Microsoft.AspNet.Identity.Owin;

using Lang = Auctioneer.Presentation.Resources.User;

namespace Auctioneer.Presentation.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserService          mUserService;
		private readonly AuthenticationManager mAuthManager;

		public UserController(IUserService userService, AuthenticationManager authManager)
		{
			Contract.Requires(userService != null);
			Contract.Requires(authManager != null);

			mUserService = userService;
			mAuthManager = authManager;
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(UserCreateViewModel input)
		{
			if(!ModelState.IsValid)
				return View(input);

			var newUser = UserCreateViewModelMapper.ToUser(input);

			await mUserService.AddUser(newUser, input.Password, new ModelStateValidationErrorNotifierAdapter(ModelState));

			if(!ModelState.IsValid)
				return View(input);

			await mAuthManager.SignInAsync(newUser, isPersistent: false, rememberBrowser: false);

			return RedirectToAction(controllerName: "Home", actionName: "Index");
		}

		public ActionResult Login(string returnUrl)
		{
			var viewModel = new UserLoginViewModel { ReturnUrl = returnUrl };

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(UserLoginViewModel input)
		{
			if(!ModelState.IsValid)
				return View(input);

			var result = await mAuthManager.PasswordSignInAsync(input.Username, input.Password, isPersistent: false,
			                                                    shouldLockout: false);
			if(result == SignInStatus.Failure)
			{
				ModelState.AddModelError(String.Empty, Lang.Login.InvalidValues);
				return View(input);
			}

			if(Url.IsLocalUrl(input.ReturnUrl))
				return Redirect(input.ReturnUrl);

			return RedirectToAction(controllerName: "Home", actionName: "Index");
		}

		public ActionResult Logout()
		{
			mAuthManager.SignOut();

			return RedirectToAction(controllerName: "Home", actionName: "Index");
		}
	}
}