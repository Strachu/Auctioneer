using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Currencies;

namespace Auctioneer.Presentation.Infrastructure.ModelBinders
{
	public class CurrencyBinder : IModelBinder
	{
		private readonly ICurrencyService mCurrencyService;

		public CurrencyBinder(ICurrencyService currencyService)
		{
			Contract.Requires(currencyService != null);

			mCurrencyService = currencyService;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if(value == null)
				return null;

			var currencySymbol = value.AttemptedValue;

			return mCurrencyService.GetCurrencyBySymbol(currencySymbol).Result;
		}
	}
}