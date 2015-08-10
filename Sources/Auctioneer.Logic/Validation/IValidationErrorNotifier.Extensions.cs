using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Auctioneer.Logic.Validation
{
	internal static class IValidationErrorNotifierExtensions
	{
		public static void AddIdentityResult(this IValidationErrorNotifier errors, IdentityResult result)
		{
			Contract.Requires(errors != null);
			Contract.Requires(result != null);

			if(result.Succeeded)
				return;

			foreach(var errorMessage in result.Errors)
			{
				errors.AddError(errorMessage);
			}
		}
	}
}
