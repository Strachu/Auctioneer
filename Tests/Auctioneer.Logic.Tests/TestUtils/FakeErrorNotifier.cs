using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Tests.TestUtils
{
	internal class FakeErrorNotifier : IValidationErrorNotifier
	{
		public void AddError(string errorMessage)
		{
			IsInErrorState = true;
		}

		public bool IsInErrorState
		{
			get;
			private set;
		}
	}
}
