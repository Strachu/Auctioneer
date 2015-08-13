using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Models.User;

namespace Auctioneer.Presentation.Mappers
{
	public class UserLockoutViewModelMapper
	{
		public static UserLockoutViewModel FromUser(User user)
		{
			Contract.Requires(user != null);

			return new UserLockoutViewModel
			{
				ExpiryDate = user.LockoutEndDateUtc.Value.ToLocalTime(),
				Reason     = user.LockoutReason
			};
		}
	}
}