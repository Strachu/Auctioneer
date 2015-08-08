using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Presentation.Infrastructure.Formatting
{
	public static class EnumExtensions
	{
		public static string GetDisplayName(this Enum value)
		{
			Contract.Requires(value != null);

			var enumType          = value.GetType();
			var enumValueInfo     = enumType.GetMember(value.ToString()).Single();
			var displayAttributes = enumValueInfo.GetCustomAttributes(typeof(DisplayAttribute), inherit: false);
			if(!displayAttributes.Any())
				return Enum.GetName(enumType, value);

			var displayAttribute  = displayAttributes.Single() as DisplayAttribute;
			return displayAttribute.GetName();
		}
	}
}
