using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Areas.Admin.Models;

namespace Auctioneer.Presentation.Areas.Admin.Mappers
{
	public class UsersIndexViewModelMapper
	{
		public static UsersIndexViewModel FromUsers(IEnumerable<User> users)
		{
			return new UsersIndexViewModel
			{
				Users = users.Select(x => new UsersIndexViewModel.Item
				{
					Id            = x.Id,
					UserName      = x.UserName,
					RealName      = x.FirstName + " " + x.LastName,
					BanExpiryTime = x.LockoutEndDateUtc
				})
			};
		}
	}
}