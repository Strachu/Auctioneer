using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic;

namespace Auctioneer.Presentation.Helpers
{
	public class ModelStateValidationErrorNotifierAdapter : IValidationErrorNotifier
	{
		private readonly ModelStateDictionary mModelState;

		public ModelStateValidationErrorNotifierAdapter(ModelStateDictionary modelState)
		{
			Contract.Requires(modelState != null);

			mModelState = modelState;
		}

		public void AddError(string errorMessage)
		{
			mModelState.AddModelError(key: String.Empty, errorMessage: errorMessage);
		}
	}
}