using System;
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
	}
}
