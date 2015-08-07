using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.Owin;

namespace Auctioneer.Presentation.Infrastructure.Security
{
	[ContractClassFor(typeof(IAuthenticationManager))]
	internal abstract class IAuthenticationManagerContractClass : IAuthenticationManager
	{
		Task<SignInStatus> IAuthenticationManager.SignIn(string userName, string password, bool rememberMe)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(userName));
			Contract.Requires(!String.IsNullOrWhiteSpace(password));

			throw new NotImplementedException();
		}

		Task IAuthenticationManager.SignOut()
		{
			throw new NotImplementedException();
		}
	}
}
