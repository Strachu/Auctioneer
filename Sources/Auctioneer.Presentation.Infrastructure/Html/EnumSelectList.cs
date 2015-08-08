using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Formatting;

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
				Text     = x.GetDisplayName(),
				Value    = x.ToString(),
				Selected = IsFlagsEnum(value.GetType()) ? value.HasFlag(x) : x.Equals(value)
			}));
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