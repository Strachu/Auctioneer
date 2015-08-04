﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Users
{
	[ContractClassFor(typeof(IUserService))]
	internal abstract class IUserServiceContractClass : IUserService
	{
		Task IUserService.AddUser(User user, string password, IValidationErrorNotifier errors)
		{
			Contract.Requires(user != null);
			Contract.Requires(password != null);
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}

		Task<User> IUserService.GetUserById(string id)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(id));

			throw new NotImplementedException();
		}

		Task<User> IUserService.GetUserByUsername(string username)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(username));

			throw new NotImplementedException();
		}
		
		Task<User> IUserService.GetUserByEmail(string email)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(email));

			throw new NotImplementedException();
		}

		Task IUserService.UpdateUser(User user)
		{
			Contract.Requires(user != null);

			throw new NotImplementedException();
		}

		Task IUserService.UpdateUserEmail(string userId,
		                                  string currentPassword,
		                                  string newEmail,
		                                  IValidationErrorNotifier errors)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(userId));
			Contract.Requires(!String.IsNullOrWhiteSpace(currentPassword));
			Contract.Requires(!String.IsNullOrWhiteSpace(newEmail));
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}

		Task IUserService.UpdateUserPassword(string userId,
		                                     string currentPassword,
		                                     string newPassword,
		                                     IValidationErrorNotifier errors)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(userId));
			Contract.Requires(!String.IsNullOrWhiteSpace(currentPassword));
			Contract.Requires(!String.IsNullOrWhiteSpace(newPassword));
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}

		Task<string> IUserService.GenerateEmailConfirmationToken(User user)
		{
			Contract.Requires(user != null);

			throw new NotImplementedException();
		}

		Task IUserService.ConfirmUserEmail(string userId, string confirmationToken, IValidationErrorNotifier errors)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(userId));
			Contract.Requires(!String.IsNullOrWhiteSpace(confirmationToken));
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}

		Task<string> IUserService.GeneratePasswordResetToken(User user)
		{
			Contract.Requires(user != null);

			throw new NotImplementedException();
		}

		Task IUserService.ResetUserPassword(string userName, string newPassword, string resetToken, IValidationErrorNotifier errors)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(userName));
			Contract.Requires(!String.IsNullOrWhiteSpace(newPassword));
			Contract.Requires(!String.IsNullOrWhiteSpace(resetToken));
			Contract.Requires(errors != null);

			throw new NotImplementedException();
		}
	}
}
