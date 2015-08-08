using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Formatting;

using NUnit.Framework;

namespace Auctioneer.Presentation.Infrastructure.Tests.Formatting
{
	internal class EnumExtensionsTests
	{
		private enum TestEnum
		{
			OptionWithoutDisplayAttribute,

			[Display(Name = "Second")]
			OptionWithDisplayAttribute,
		}

		[Test]
		public void WhenValueDoesNotHaveDisplayAttribute_GetDisplayName_ReturnsTheValueName()
		{
			var returnedName = TestEnum.OptionWithoutDisplayAttribute.GetDisplayName();

			Assert.That(returnedName, Is.EqualTo("OptionWithoutDisplayAttribute"));
		}

		[Test]
		public void WhenValueHasDisplayAttribute_GetDisplayName_ReadsTheNameFromIt()
		{
			var returnedName = TestEnum.OptionWithDisplayAttribute.GetDisplayName();

			Assert.That(returnedName, Is.EqualTo("Second"));
		}
	}
}
