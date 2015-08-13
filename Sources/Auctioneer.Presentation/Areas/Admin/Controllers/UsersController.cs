using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Areas.Admin.Mappers;
using Auctioneer.Presentation.Areas.Admin.Models;
using Auctioneer.Presentation.Infrastructure.Formatting;
using Auctioneer.Presentation.Infrastructure.Http;

using Lang = Auctioneer.Resources.Admin.Users;

namespace Auctioneer.Presentation.Areas.Admin.Controllers
{
	[Authorize(Roles="Admin")]
	public class UsersController : Controller
	{
		private readonly IUserService mUserService;

		public UsersController(IUserService userService)
		{
			Contract.Requires(userService != null);

			mUserService = userService;
		}

		public async Task<ActionResult> Index(UserSortOrder sortOrder = UserSortOrder.UserNameAscending,
		                                      int page = 1)
		{
			var users     = await mUserService.GetAllUsers(sortOrder, page, usersPerPage: 50);
			var viewModel = UsersIndexViewModelMapper.FromUsers(users, sortOrder);

			ViewBag.Message = TempData["Message"];
			return View(viewModel);
		}

		public async Task<ActionResult> Ban(string id, string listQueryString)
		{
			var user      = await mUserService.GetUserById(id);
			var viewModel = new UsersBanViewModel
			{
				Id              = user.Id,
				ListQueryString = listQueryString,
				UserName        = user.UserName,
				RealName        = user.FullName
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Ban(UsersBanViewModel input)
		{
			input.Reason = input.Reason ?? "No reason";

			await mUserService.BanUser(input.Id, input.BanDuration, input.Reason);

			var confirmationMessage = String.Format(Lang.Ban.ConfirmationMessage, input.UserName,
			                                        TimeSpanFormatter.Format(input.BanDuration));

			return RedirectToListWithMessage(input.ListQueryString, confirmationMessage);
		}

		public async Task<ActionResult> Unban(string id, string listQueryString)
		{
			var user      = await mUserService.GetUserById(id);
			var viewModel = UsersUnbanViewModelMapper.FromUser(user, listQueryString);

			return View(viewModel);
		}

		[HttpPost]
		[ActionName("Unban")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UnbanPost(string id, string userName, string listQueryString)
		{
			await mUserService.UnbanUser(id);

			var confirmationMessage = String.Format(Lang.Unban.ConfirmationMessage, userName);

			return RedirectToListWithMessage(listQueryString, confirmationMessage);
		}

		private ActionResult RedirectToListWithMessage(string queryString, string message)
		{
			TempData["Message"] = message;

			if(String.IsNullOrWhiteSpace(queryString))
				return RedirectToAction("Index");
			else
				return RedirectToAction("Index", HttpUtility.ParseQueryString(queryString).ToRouteDictionary());
		}
	}
}