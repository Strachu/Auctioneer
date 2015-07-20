using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Presentation.Helpers;

using NUnit.Framework;

namespace Auctioneer.Presentation.Tests.Helpers
{
	[TestFixture]
	public class TimeSpanFormatterTests
	{
		[Test]
		public void WriteDaysIfTimeSpanIsMoreThanOneDay()
		{
			var timeSpan = new TimeSpan(days: 2, hours: 3, minutes: 43, seconds: 27);

			var result = TimeSpanFormatter.Format(timeSpan);

			Assert.That(result, Is.EqualTo("2 days"));
		}

		[Test]
		public void WriteHoursIfTimeSpanIsMoreThanOneHourButLessThanDay()
		{
			var timeSpan = new TimeSpan(days: 0, hours: 22, minutes: 43, seconds: 27);

			var result = TimeSpanFormatter.Format(timeSpan);

			Assert.That(result, Is.EqualTo("22 hours"));
		}

		[Test]
		public void WriteMinutesIfTimeSpanIsMoreThanOneMinuteButLessThanHour()
		{
			var timeSpan = new TimeSpan(days: 0, hours: 0, minutes: 43, seconds: 27);

			var result = TimeSpanFormatter.Format(timeSpan);

			Assert.That(result, Is.EqualTo("43 minutes"));
		}

		[Test]
		public void WriteLessThanMinuteIfTimeSpanIsItsTrue()
		{
			var timeSpan = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 33);

			var result = TimeSpanFormatter.Format(timeSpan);

			Assert.That(result, Is.EqualTo("Less than a minute"));
		}
	}
}
