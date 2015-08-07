using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.ModelBinders;

namespace Auctioneer.Presentation
{
	public class ModelBinderConfig
	{
		public static void RegisterModelBinders()
		{
			ModelBinderProviders.BinderProviders.Add(new FlagsEnumModelBinderProvider());
		}
	}
}
