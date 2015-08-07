using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Infrastructure.ModelBinders
{
	public class FlagsEnumModelBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var values = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if(values == null)
				return null;

			return Enum.Parse(bindingContext.ModelType, values.AttemptedValue);
		}
	}

	public class FlagsEnumModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(Type modelType)
		{
			bool isEnum = typeof(Enum).IsAssignableFrom(modelType);
			if(!isEnum)
				return null;

			bool hasFlagsAttribute = modelType.GetCustomAttributes(typeof(FlagsAttribute), inherit: true).Any();
			if(!hasFlagsAttribute)
				return null;

			return new FlagsEnumModelBinder();
		}
	}
}