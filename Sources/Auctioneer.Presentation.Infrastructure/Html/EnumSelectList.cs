using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	public class EnumSelectList : Collection<SelectListItem>
	{
		private EnumSelectList(IEnumerable<SelectListItem> items) : base(items.ToList())
		{
		}

		public static EnumSelectList FromSelectedValue(Enum value)
		{
			Contract.Requires(value != null);

			var allValues    = Enum.GetValues(value.GetType()).Cast<Enum>();
			var valuesToShow = !IsFlagsEnum(value.GetType()) ? allValues
			                                                 : allValues.Where(x => IsPowerOfTwo(Convert.ToUInt64(x)));

			return new EnumSelectList(valuesToShow.Select(x => new SelectListItem
			{
				Text     = GetNameOfEnumValue(x),
				Value    = x.ToString(),
				Selected = IsFlagsEnum(value.GetType()) ? value.HasFlag(x) : x.Equals(value)
			}));
		}

		private static string GetNameOfEnumValue(Enum value)
		{
			var enumType          = value.GetType();
			var enumValueInfo     = enumType.GetMember(value.ToString()).Single();
			var displayAttributes = enumValueInfo.GetCustomAttributes(typeof(DisplayAttribute), inherit: false);
			if(!displayAttributes.Any())
				return Enum.GetName(enumType, value);

			var displayAttribute  = displayAttributes.Single() as DisplayAttribute;
			return displayAttribute.GetName();
		}

		private static bool IsFlagsEnum(Type enumType)
		{
			var flagsAttributes = enumType.GetCustomAttributes(typeof(FlagsAttribute), inherit: false);
			return flagsAttributes.Any();
		}

		private static bool IsPowerOfTwo(ulong value)
		{
			for(int i = 0; i < 64; ++i)
			{
				if(value == (1ul << i))
					return true;
			}

			return false;
		}
	}
}