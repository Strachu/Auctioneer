using System;
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
	}
}
