using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Emails;
using Auctioneer.Presentation.Infrastructure.Security;
using Auctioneer.Presentation.Infrastructure.Validation;
using Auctioneer.Presentation.Mappers;
using Auctioneer.Presentation.Models.User;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Lang = Auctioneer.Resources.User;

namespace Auctioneer.Presentation.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserService           mUserService;
		private readonly IAuthenticationManager mAuthManager;

		public UserController(IUserService userService, IAuthenticationManager authManager)
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

			await mUserService.AddUser(newUser, input.Password, new ValidationErrorNotifierAdapter(ModelState));
			if(!ModelState.IsValid)
				return View(input);

			return RedirectToAction("AccountCreated");
		}

		public ActionResult AccountCreated()
		{
			return View();
		}

		public async Task<ActionResult> ConfirmMail(string userId, string confirmToken)
		{
			if(String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(confirmToken))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			await mUserService.ConfirmUserEmail(userId, confirmToken, new ValidationErrorNotifierAdapter(ModelState));
			if(!ModelState.IsValid)
				return View("Error");

			return View();
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

			var user = await mUserService.GetUserByUsername(input.Username);

			bool wasBannedBeforeLoginAttempt = user.IsBanned;

			var result = await mAuthManager.SignIn(input.Username, input.Password, input.RememberMe);
			if(result == SignInStatus.Failure)
			{
				ModelState.AddModelError(String.Empty, Lang.Login.InvalidValues);
				return View(input);
			}

			if(result == SignInStatus.LockedOut)
			{
				if(!wasBannedBeforeLoginAttempt)
				{
					user.LockoutReason = Lang.Lockout.TooManyInvalidLoginAttempts;

					await mUserService.UpdateUser(user);
				}

				return RedirectToAction("Lockout", new { id = user.Id });
			}

			if(!user.EmailConfirmed)
			{
				await mAuthManager.SignOut();

				ViewBag.UserId = user.Id;
				return View("EmailNotConfirmed");
			}

			if(Url.IsLocalUrl(input.ReturnUrl))
				return Redirect(input.ReturnUrl);

			return RedirectToAction(controllerName: "Home", actionName: "Index");
		}

		public async Task<ActionResult> Lockout(string id)
		{
			if(String.IsNullOrWhiteSpace(id))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var user      = await mUserService.GetUserById(id);
			var viewModel = UserLockoutViewModelMapper.FromUser(user);

			return View(viewModel);
		}

		public async Task<ActionResult> ResendMailConfirmation(string userId)
		{
			if(String.IsNullOrWhiteSpace(userId))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			await mUserService.SendActivationToken(userId);

			return View();
		}

		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(UserForgotPasswordViewModel input)
		{
			if(!ModelState.IsValid)
				return View(input);

			var user = await mUserService.GetUserByEmail(input.Email);

			await mUserService.SendPasswordResetToken(user);

			return RedirectToAction("ResetPasswordMailSent");
		}

		public ActionResult ResetPasswordMailSent()
		{
			return View();
		}

		public ActionResult ResetPassword(string resetToken)
		{
			if(String.IsNullOrWhiteSpace(resetToken))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var viewModel = new UserResetPasswordViewModel { ResetToken = resetToken };
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(UserResetPasswordViewModel input)
		{
			if(!ModelState.IsValid)
				return View(input);

			await mUserService.ResetUserPassword(input.Username, input.NewPassword, input.ResetToken, 
			                                     new ValidationErrorNotifierAdapter(ModelState));

			if(!ModelState.IsValid)
				return View(input);

			return RedirectToAction("ResetPasswordConfirmation");
		}

		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		public async Task<ActionResult> Logout()
		{
			await mAuthManager.SignOut();

			return RedirectToAction(controllerName: "Home", actionName: "Index");
		}

		[ChildActionOnly]
		public ActionResult Greetings()
		{
			if(User.Identity.IsAuthenticated)
			{
				var user = mUserService.GetUserById(User.Identity.GetUserId()).Result;

				return PartialView("_Greetings", user);
			}

			return new EmptyResult();
		}
	}
}