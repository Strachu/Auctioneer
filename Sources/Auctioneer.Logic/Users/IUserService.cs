﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Users
{
	[ContractClass(typeof(IUserServiceContractClass))]
	public interface IUserService
	{
		Task AddUser(User user, string password, IValidationErrorNotifier errors);

		Task<User> GetUserById(string id);
		Task<User> GetUserByUsername(string username);
		Task<User> GetUserByEmail(string email);

		Task<string> GenerateEmailConfirmationToken(User user);
		Task ConfirmUserEmail(string userId, string confirmationToken, IValidationErrorNotifier errors);

		Task<string> GeneratePasswordResetToken(User user);
		Task ResetUserPassword(string userName, string newPassword, string resetToken, IValidationErrorNotifier errors);
	}
}
