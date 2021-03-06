﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Validation;

using PagedList;

namespace Auctioneer.Logic.Users
{
	[ContractClass(typeof(IUserServiceContractClass))]
	public interface IUserService
	{
		Task AddUser(User user, string password, IValidationErrorNotifier errors);

		Task<IPagedList<User>> GetAllUsers(UserSortOrder sortOrder, int page, int usersPerPage);

		Task<User> GetUserById(string id);
		Task<User> GetUserByUsername(string username);
		Task<User> GetUserByEmail(string email);

		Task UpdateUser(User user);
		Task UpdateUserEmail(string userId, string currentPassword, string newEmail, IValidationErrorNotifier errors);
		Task UpdateUserPassword(string userId, string currentPassword, string newPassword, IValidationErrorNotifier errors);

		Task SendActivationToken(string userId);
		Task ConfirmUserEmail(string userId, string confirmationToken, IValidationErrorNotifier errors);

		Task SendPasswordResetToken(User user);
		Task ResetUserPassword(string userName, string newPassword, string resetToken, IValidationErrorNotifier errors);

		Task BanUser(string userId, TimeSpan banDuration, string reason);
		Task UnbanUser(string userId);

		Task<bool> IsUserInRole(string userId, string role);
	}
}
