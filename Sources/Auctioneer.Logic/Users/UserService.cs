using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Auctioneer.Logic.Users
{
	public class UserService : UserManager<User>, IUserService
	{
		public UserService(AuctioneerDbContext context) : base(new UserStore<User>(context))
		{
			Contract.Requires(context != null);

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

		public async Task<User> GetUserById(string id)
		{
			var user = await base.FindByIdAsync(id);
			if(user == null)
				throw new ObjectNotFoundException("User with id = " + id + " does not exist.");

			return user;
		}

		public async Task<User> GetUserByUsername(string username)
		{
			var user = await base.FindByNameAsync(username);
			if(user == null)
				throw new ObjectNotFoundException("User with user name = " + username + " does not exist.");

			return user;
		}

		public async Task<User> GetUserByEmail(string email)
		{
			var user = await base.FindByEmailAsync(email);
			if(user == null)
				throw new ObjectNotFoundException("User with e-mail = " + email + " does not exist.");

			return user;
		}

		public async Task AddUser(User user, string password, IValidationErrorNotifier errors)
		{
			var result = await base.CreateAsync(user, password);

			errors.AddIdentityResult(result);
		}

		public Task<string> GenerateEmailConfirmationToken(User user)
		{
			return base.GenerateEmailConfirmationTokenAsync(user.Id);
		}

		public async Task ConfirmUserEmail(string userId, string confirmationToken, IValidationErrorNotifier errors)
		{
			var result = await base.ConfirmEmailAsync(userId, confirmationToken);

			errors.AddIdentityResult(result);
		}

		public Task<string> GeneratePasswordResetToken(User user)
		{
			return base.GeneratePasswordResetTokenAsync(user.Id);
		}

		public async Task ResetUserPassword(string userName, string newPassword, string resetToken, IValidationErrorNotifier errors)
		{
			var user   = await this.GetUserByUsername(userName);
			var result = await base.ResetPasswordAsync(user.Id, resetToken, newPassword);

			errors.AddIdentityResult(result);
		}
	}
}
