using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Html;

namespace Auctioneer.Presentation.Infrastructure.ModelBinders
{
	public class DurationModelBinder : DefaultModelBinder
	{
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if(bindingContext.ModelMetadata.DataTypeName != DataType.Duration.ToString())
				return base.BindModel(controllerContext, bindingContext);

			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			var unit  = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Unit");
			if(value == null || unit == null)
				return null;

			var valueAsDouble = Double.Parse(value.AttemptedValue);
			var unitAsEnum    = (DurationUnit)Enum.Parse(typeof(DurationUnit), unit.AttemptedValue);

			switch(unitAsEnum)
			{
				default: //case DurationUnit.Days:
					return TimeSpan.FromDays(valueAsDouble);

				case DurationUnit.Hours:
					return TimeSpan.FromHours(valueAsDouble);

				case DurationUnit.Minutes:
					return TimeSpan.FromMinutes(valueAsDouble);
			}
		}
	}
}
