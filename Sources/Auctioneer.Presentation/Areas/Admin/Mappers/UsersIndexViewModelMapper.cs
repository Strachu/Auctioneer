using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Areas.Admin.Models;

using PagedList;

namespace Auctioneer.Presentation.Areas.Admin.Mappers
{
	public class UsersIndexViewModelMapper
	{
		public static UsersIndexViewModel FromUsers(IPagedList<User> users)
		{
			Contract.Requires(users != null);

			var items = users.Select(x => new UsersIndexViewModel.Item
			{
				Id            = x.Id,
				UserName      = x.UserName,
				RealName      = x.FirstName + " " + x.LastName,
				BanExpiryTime = x.LockoutEndDateUtc
			});

			return new UsersIndexViewModel
			{
				Users = new StaticPagedList<UsersIndexViewModel.Item>(items, users),
			};
		}
	}
}