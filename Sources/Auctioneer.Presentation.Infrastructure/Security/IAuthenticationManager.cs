using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.Owin;

namespace Auctioneer.Presentation.Infrastructure.Security
{
	[ContractClass(typeof(IAuthenticationManagerContractClass))]
	public interface IAuthenticationManager
	{
		Task<SignInStatus> SignIn(string userName, string password, bool rememberMe);

		Task SignOut();
	}
}
