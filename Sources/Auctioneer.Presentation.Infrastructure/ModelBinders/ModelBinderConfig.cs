using System;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Infrastructure.ModelBinders
{
	public class ModelBinderConfig
	{
		public static void RegisterModelBinders()
		{
			ModelBinderProviders.BinderProviders.Add(new FlagsEnumModelBinderProvider());
			System.Web.Mvc.ModelBinders.Binders.Add(typeof(TimeSpan), new DurationModelBinder());
		}
	}
}
