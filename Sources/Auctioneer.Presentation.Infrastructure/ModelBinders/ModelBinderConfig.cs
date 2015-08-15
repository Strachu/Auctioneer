using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Currencies;

namespace Auctioneer.Presentation.Infrastructure.ModelBinders
{
	public class ModelBinderConfig
	{
		public static void RegisterModelBinders()
		{
			ModelBinderProviders.BinderProviders.Add(new FlagsEnumModelBinderProvider());

			var currencyService = DependencyResolver.Current.GetService<ICurrencyService>();
			System.Web.Mvc.ModelBinders.Binders.Add(typeof(Currency), new CurrencyBinder(currencyService));
		}
	}
}
