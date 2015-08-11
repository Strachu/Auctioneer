using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Areas.Admin.Mappers;

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

		public async Task<ActionResult> Index(int page = 1)
		{
			var users     = await mUserService.GetAllUsers(page, usersPerPage: 50);
			var viewModel = UsersIndexViewModelMapper.FromUsers(users);

			return View(viewModel);
		}
	}
}