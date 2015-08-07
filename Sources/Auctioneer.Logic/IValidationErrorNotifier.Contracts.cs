using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic
{
	[ContractClassFor(typeof(IValidationErrorNotifier))]
	internal abstract class IValidationErrorNotifierContractClass : IValidationErrorNotifier
	{
		void IValidationErrorNotifier.AddError(string errorMessage)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(errorMessage));

			throw new NotImplementedException();
		}
	}
}
