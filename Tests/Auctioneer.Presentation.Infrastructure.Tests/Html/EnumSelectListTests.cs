using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Html;

using NUnit.Framework;

namespace Auctioneer.Presentation.Infrastructure.Tests.Html
{
	internal class EnumSelectListTests
	{
		private enum SimpleEnum
		{
			Option1,
			Option2,
			Option3
		}

		[Test]
		public void AllValuesInSimpleEnumAreReturned()
		{
			var enumList = EnumSelectList.FromSelectedValue(SimpleEnum.Option2);

			AssertThatTextAndValuesAreEqualInBothCollection(enumList, new SelectListItem[]
			{
				new SelectListItem { Text = "Option1", Value = "Option1" },
				new SelectListItem { Text = "Option2", Value = "Option2" },
				new SelectListItem { Text = "Option3", Value = "Option3" },
			});
		}

		[Test]
		public void PassedOptionFromSimpleEnumIsSelected()
		{
			var enumList = EnumSelectList.FromSelectedValue(SimpleEnum.Option2);

			AssertSelectedValuesAreEqualTo(enumList, SimpleEnum.Option2);
		}

		private enum SimpleEnumWithDisplayAttribute
		{
			[Display(Name = "First")]
			Option1,

			[Display(Name = "Second")]
			Option2,

			[Display(Name = "Third")]
			Option3
		}

		[Test]
		public void WhenDisplayAttributeIsSpecified_ItIsUsedToDetermineEnumName()
		{
			var enumList = EnumSelectList.FromSelectedValue(SimpleEnumWithDisplayAttribute.Option2);

			AssertThatTextAndValuesAreEqualInBothCollection(enumList, new SelectListItem[]
			{
				new SelectListItem { Text = "First",  Value = "Option1" },
				new SelectListItem { Text = "Second", Value = "Option2" },
				new SelectListItem { Text = "Third",  Value = "Option3" },
			});
		}

		[Flags]
		private enum FlagsEnum
		{
			Option1         = 1 << 0,
			Option2         = 1 << 1,
			Option3         = 1 << 2,

			CompoundOption1 = Option1 | Option2,
			CompoundOption2 = Option1 | Option2 | Option3
		}

		[Test]
		public void WhenFlagsEnumIsUsed_CompositeValuesAreIgnored()
		{
			var enumList = EnumSelectList.FromSelectedValue(FlagsEnum.Option2);

			AssertThatTextAndValuesAreEqualInBothCollection(enumList, new SelectListItem[]
			{
				new SelectListItem { Text = "Option1", Value = "Option1" },
				new SelectListItem { Text = "Option2", Value = "Option2" },
				new SelectListItem { Text = "Option3", Value = "Option3" },
			});
		}

		[Test]
		public void WhenFlagsEnumIsUsed_CompositeValueSelectsMultipleSingleValues()
		{
			var enumList = EnumSelectList.FromSelectedValue(FlagsEnum.Option2 | FlagsEnum.Option3);

			AssertSelectedValuesAreEqualTo(enumList, SimpleEnum.Option2, SimpleEnum.Option3);
		}

		private void AssertThatTextAndValuesAreEqualInBothCollection(IReadOnlyCollection<SelectListItem> values,
		                                                             IReadOnlyCollection<SelectListItem> expected)
		{
			Assert.That(values.Count(), Is.EqualTo(expected.Count()), "Invalid number of elements");

			var valuesToCompare = values.Zip(expected, (x1, x2) => new
			{
				Value    = x1,
				Expected = x2
			});

			foreach(var valueToCompare in valuesToCompare)
			{
				Assert.That(valueToCompare.Value.Text,  Is.EqualTo(valueToCompare.Expected.Text));
				Assert.That(valueToCompare.Value.Value, Is.EqualTo(valueToCompare.Expected.Value));
			}
		}

		private void AssertSelectedValuesAreEqualTo<TEnum>(IEnumerable<SelectListItem> list, params TEnum[] expected)
		{
			var selectedValues = list.Where(x => x.Selected).Select(x => x.Value);
			var expectedValues = expected.Select(x => x.ToString());

			Assert.That(selectedValues, Is.EquivalentTo(expectedValues));
		}
	}
}
