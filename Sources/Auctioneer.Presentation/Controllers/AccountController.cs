using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Mappers.Account;

using Microsoft.AspNet.Identity;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Models.Account;

namespace Auctioneer.Presentation.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly IUserService mUserService;

		public AccountController(IUserService userService)
		{
			Contract.Requires(userService != null);

			mUserService = userService;
		}

		public Task<ViewResult> Index()
		{
			return Edit();
		}

		public async Task<ViewResult> Edit()
		{
			var user      = await mUserService.GetUserById(User.Identity.GetUserId());
			var viewModel = AccountEditViewModelMapper.FromUser(user);

			return View("Edit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditLoginDetails(AccountEditViewModel.LoginDetailsViewModel input)
		{
			if(!ModelState.IsValid)
				return await FullEditView(model => model.LoginDetails = input.WithValidationSummary());

			var userId = User.Identity.GetUserId();

			using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				await mUserService.UpdateUserEmail(userId, input.CurrentPassword, input.Email,
					                                new ValidationErrorNotifierAdapter(ModelState));
				if(!ModelState.IsValid)
					return await FullEditView(model => model.LoginDetails = input.WithValidationSummary());

				if(!String.IsNullOrWhiteSpace(input.NewPassword))
				{
					await mUserService.UpdateUserPassword(userId, input.CurrentPassword, input.NewPassword,
					                                      new ValidationErrorNotifierAdapter(ModelState));

					if(!ModelState.IsValid)
						return await FullEditView(model => model.LoginDetails = input.WithValidationSummary());
				}

				transaction.Complete();
			}

			return RedirectToAction("AccountDetailsUpdated");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditPersonalInfo(AccountEditViewModel.PersonalInfoViewModel input)
		{
			if(!ModelState.IsValid)
				return await FullEditView(model => model.PersonalInfo = input.WithValidationSummary());

			var user = await mUserService.GetUserById(User.Identity.GetUserId());

			user.FirstName = input.FirstName;
			user.LastName  = input.LastName;
			user.Address   = input.Address;

			await mUserService.UpdateUser(user);

			return RedirectToAction("AccountDetailsUpdated");
		}

		private async Task<ViewResult> FullEditView(Action<AccountEditViewModel> modelModifier)
		{
			var editView  = await Edit();
			var fullModel = (AccountEditViewModel)editView.Model;

			modelModifier(fullModel);

			return editView;			
		}

		public ActionResult AccountDetailsUpdated()
		{
			return View();
		}
	}
}