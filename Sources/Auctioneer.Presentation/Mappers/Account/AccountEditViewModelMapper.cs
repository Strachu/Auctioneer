using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;
using Auctioneer.Presentation.Models.Account;

namespace Auctioneer.Presentation.Mappers.Account
{
	public class AccountEditViewModelMapper
	{
		public static AccountEditViewModel FromUser(User user)
		{
			Contract.Requires(user != null);

			return new AccountEditViewModel
			{
				LoginDetails = new AccountEditViewModel.LoginDetailsViewModel
				{
					Email = user.Email
				},

				PersonalInfo = new AccountEditViewModel.PersonalInfoViewModel
				{
					FirstName = user.FirstName,
					LastName  = user.LastName,
					Address   = user.Address
				}
			};
		}
	}
}