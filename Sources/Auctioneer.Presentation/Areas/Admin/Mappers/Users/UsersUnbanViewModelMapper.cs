using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Areas.Admin.Models;

namespace Auctioneer.Presentation.Areas.Admin.Mappers
{
	public class UsersUnbanViewModelMapper
	{
		public static UsersUnbanViewModel FromUser(User user, string listQueryString)
		{
			Contract.Requires(user != null);

			return new UsersUnbanViewModel
			{
				Id              = user.Id,
				UserName        = user.UserName,
				RealName        = user.FullName,
				ListQueryString = listQueryString,
				Reason          = user.LockoutReason,
				TimeTillBanEnd  = user.LockoutEndDateUtc.Value.Subtract(DateTime.UtcNow)
			};
		}
	}
}