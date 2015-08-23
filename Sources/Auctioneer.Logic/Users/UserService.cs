using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using EntityFramework.Extensions;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Validation;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

using PagedList;

using Lang = Auctioneer.Resources.Account;

namespace Auctioneer.Logic.Users
{
	public class UserService : UserManager<User>, IUserService
	{
		private readonly AuctioneerDbContext mContext;
		private readonly IUserNotifier       mUserNotifier;

		public UserService(AuctioneerDbContext context, IUserNotifier userNotifier) : base(new UserStore<User>(context))
		{
			Contract.Requires(context != null);
			Contract.Requires(userNotifier != null);

			mContext      = context;
			mUserNotifier = userNotifier;

			base.PasswordValidator = new PasswordValidator
			{
				RequiredLength          = 1,
				RequireDigit            = false,
				RequireLowercase        = false,
				RequireNonLetterOrDigit = false,
				RequireUppercase        = false
			};
			
			base.UserValidator = new UserValidator<User>(this)
			{
				AllowOnlyAlphanumericUserNames = true,
				RequireUniqueEmail             = true
			};

			base.UserLockoutEnabledByDefault          = true;
			base.MaxFailedAccessAttemptsBeforeLockout = 5;
			base.DefaultAccountLockoutTimeSpan        = TimeSpan.FromMinutes(10);

			// http://stackoverflow.com/questions/22629936/no-iusertokenprovider-is-registered
			var protectionProvider = new DpapiDataProtectionProvider("Auctioneer");
			base.UserTokenProvider = new DataProtectorTokenProvider<User>(protectionProvider.Create("Auctioneer_Tokens"));
		}

		public async Task AddUser(User user, string password, IValidationErrorNotifier errors)
		{
			var result = await base.CreateAsync(user, password);
			if(!result.Succeeded)
			{
				errors.AddIdentityResult(result);
				return;
			}

			await SendActivationToken(user.Id);
		}

		public Task<IPagedList<User>> GetAllUsers(UserSortOrder sortOrder, int page, int usersPerPage)
		{
			var users = base.Users;

			switch(sortOrder)
			{
				default: //UserSortOrder.UserNameAscending:
					users = users.OrderBy(x => x.UserName);
					break;
				case UserSortOrder.UserNameDescending:
					users = users.OrderByDescending(x => x.UserName);
					break;

				case UserSortOrder.RealNameAscending:
					users = users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
					break;
				case UserSortOrder.RealNameDescending:
					users = users.OrderByDescending(x => x.LastName).ThenBy(x => x.FirstName);
					break;

				case UserSortOrder.BanExpiryDateAscending:
					users = users.OrderBy(x => x.LockoutEndDateUtc);
					break;
				case UserSortOrder.BanExpiryDateDescending:
					users = users.OrderByDescending(x => x.LockoutEndDateUtc);
					break;
			}

			return Task.FromResult(users.ToPagedList(page, usersPerPage));
		}

		public async Task<User> GetUserById(string id)
		{
			var user = await base.FindByIdAsync(id).ConfigureAwait(false);
			if(user == null)
				throw new ObjectNotFoundException("User with id = " + id + " does not exist.");

			return user;
		}

		public async Task<User> GetUserByUsername(string username)
		{
			var user = await base.FindByNameAsync(username).ConfigureAwait(false);
			if(user == null)
				throw new ObjectNotFoundException("User with user name = " + username + " does not exist.");

			return user;
		}

		public async Task<User> GetUserByEmail(string email)
		{
			var user = await base.FindByEmailAsync(email).ConfigureAwait(false);
			if(user == null)
				throw new ObjectNotFoundException("User with e-mail = " + email + " does not exist.");

			return user;
		}

		public Task UpdateUser(User user)
		{
			return base.UpdateAsync(user);
		}

		public async Task UpdateUserEmail(string userId,
		                                  string currentPassword,
		                                  string newEmail,
		                                  IValidationErrorNotifier errors)
		{
			var user              = await this.GetUserById(userId);
			var passwordIsCorrect = await base.CheckPasswordAsync(user, currentPassword);
			if(!passwordIsCorrect)
			{
				errors.AddError(Lang.Edit.InvalidPassword);
				return;
			}

			var result = await base.SetEmailAsync(userId, newEmail);
			
			// TODO Is mail confirmation needed? If so, send confirmation e-mail

			errors.AddIdentityResult(result);
		}

		public async Task UpdateUserPassword(string userId,
		                                     string currentPassword,
		                                     string newPassword,
		                                     IValidationErrorNotifier errors)
		{
			var result = await base.ChangePasswordAsync(userId, currentPassword, newPassword);

			errors.AddIdentityResult(result);
		}

		public async Task SendActivationToken(string userId)
		{
			var user            = await this.GetUserById(userId);
			var activationToken = await base.GenerateEmailConfirmationTokenAsync(userId);

			await mUserNotifier.SendActivationToken(user, activationToken);
		}

		public async Task ConfirmUserEmail(string userId, string confirmationToken, IValidationErrorNotifier errors)
		{
			var result = await base.ConfirmEmailAsync(userId, confirmationToken).ConfigureAwait(false);

			errors.AddIdentityResult(result);
		}

		public async Task SendPasswordResetToken(User user)
		{
			var token = await base.GeneratePasswordResetTokenAsync(user.Id);

			await mUserNotifier.SendPasswordResetToken(user, token);
		}

		public async Task ResetUserPassword(string userName, string newPassword, string resetToken, IValidationErrorNotifier errors)
		{
			var user   = await this.GetUserByUsername(userName).ConfigureAwait(false);
			var result = await base.ResetPasswordAsync(user.Id, resetToken, newPassword).ConfigureAwait(false);

			errors.AddIdentityResult(result);
		}

		public async Task BanUser(string userId, TimeSpan banDuration, string reason)
		{
			var user = await this.GetUserById(userId);

			using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				user.LockoutEndDateUtc = DateTime.UtcNow.Add(banDuration);
				user.LockoutReason     = reason;

				await base.UpdateAsync(user);

				await mContext.Auctions.Where(x => x.SellerId == userId)
				                       .Where(AuctionStatusFilter.Active)
				                       .UpdateAsync(x => new Auction
				                       {
					                       EndDate = DateTime.Now
				                       });

				transaction.Complete();
			}
		}

		public async Task UnbanUser(string userId)
		{
			var user = await this.GetUserById(userId);

			user.LockoutEndDateUtc = null;
			user.LockoutReason     = null;

			await base.UpdateAsync(user);
		}

		public Task<bool> IsUserInRole(string userId, string role)
		{
			return base.IsInRoleAsync(userId, role);
		}
	}
}
