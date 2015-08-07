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

using Postal;

using Lang = Auctioneer.Presentation.Resources.User;

namespace Auctioneer.Presentation.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserService           mUserService;
		private readonly IEmailService          mMailService;
		private readonly IAuthenticationManager mAuthManager;

		public UserController(IUserService userService, IEmailService mailService, IAuthenticationManager authManager)
		{
			Contract.Requires(userService != null);
			Contract.Requires(mailService != null);
			Contract.Requires(authManager != null);

			mUserService = userService;
			mMailService = mailService;
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

			await SendConfirmationMail(newUser);

			return RedirectToAction("AccountCreated");
		}

		private async Task SendConfirmationMail(User userToSentTo)
		{
			var mailConfirmationToken = await mUserService.GenerateEmailConfirmationToken(userToSentTo);

			await mMailService.SendAsync(new ConfirmationMail
			{
				UserMail      = userToSentTo.Email,
				UserFirstName = userToSentTo.FirstName,
				UserId        = userToSentTo.Id,
				ConfirmToken  = mailConfirmationToken
			});
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

			var result = await mAuthManager.SignIn(input.Username, input.Password, input.RememberMe);
			if(result == SignInStatus.Failure)
			{
				ModelState.AddModelError(String.Empty, Lang.Login.InvalidValues);
				return View(input);
			}

			if(result == SignInStatus.LockedOut)
				return View("LockedOut");

			var user = await mUserService.GetUserByUsername(input.Username);
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

		public async Task<ActionResult> ResentMailConfirmation(string userId)
		{
			if(String.IsNullOrWhiteSpace(userId))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var user = await mUserService.GetUserById(userId);

			await SendConfirmationMail(user);

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

			var user               = await mUserService.GetUserByEmail(input.Email);
			var passwordResetToken = await mUserService.GeneratePasswordResetToken(user);

			await mMailService.SendAsync(new ForgotPasswordMail
			{
				UserMail           = user.Email,
				UserFirstName      = user.FirstName,
				PasswordResetToken = passwordResetToken
			});

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