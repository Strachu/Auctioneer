using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Validation
{
	[ContractClass(typeof(IValidationErrorNotifierContractClass))]
	public interface IValidationErrorNotifier
	{
		void AddError(string errorMessage);
	}
}
