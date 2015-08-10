using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Utils
{
	internal class ErrorCollection : Collection<string>, IValidationErrorNotifier
	{
		public void AddError(string errorMessage)
		{
			base.Add(errorMessage);
		}
	}
}
