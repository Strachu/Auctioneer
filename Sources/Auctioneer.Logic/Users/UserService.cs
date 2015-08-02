using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
		}

		public async Task AddUser(User user, string password, IValidationErrorNotifier errors)
		{
			var result = await base.CreateAsync(user, password);

			errors.AddIdentityResult(result);

			// TODO send confirmation mail
		}
	}
}
