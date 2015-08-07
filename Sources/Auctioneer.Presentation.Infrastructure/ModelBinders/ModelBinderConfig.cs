using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Infrastructure.ModelBinders
{
	public class ModelBinderConfig
	{
		public static void RegisterModelBinders()
		{
			ModelBinderProviders.BinderProviders.Add(new FlagsEnumModelBinderProvider());
		}
	}
}
