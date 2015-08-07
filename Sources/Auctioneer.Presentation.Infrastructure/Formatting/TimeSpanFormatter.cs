using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Resources.Other;

namespace Auctioneer.Presentation.Infrastructure.Formatting
{
	public static class TimeSpanFormatter
	{
		public static string Format(TimeSpan value)
		{
			if(value.TotalDays >= 1.0)
				return String.Format("{0} {1}", value.Days, Lang.TimeSpan_Days);

			if(value.TotalHours >= 1.0)
				return String.Format("{0} {1}", value.Hours, Lang.TimeSpan_Hours);

			if(value.TotalMinutes >= 1.0)
				return String.Format("{0} {1}", value.Minutes, Lang.TimeSpan_Minutes);

			return Lang.TimeSpan_LessThanMinute;
		}
	}
}