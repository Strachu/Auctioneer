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
	public class UserCreateViewModelMapper
	{
		public static User ToUser(UserCreateViewModel viewModel)
		{
			Contract.Requires(viewModel != null);

			return new User
			{
				UserName     = viewModel.Username,
				Email        = viewModel.Email,
				FirstName    = viewModel.FirstName,
				LastName     = viewModel.LastName,
				Address      = viewModel.Address
			};
		}
	}
}