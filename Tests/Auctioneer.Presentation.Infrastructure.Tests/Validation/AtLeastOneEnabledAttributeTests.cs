using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Presentation.Infrastructure.Validation;

using NUnit.Framework;

namespace Auctioneer.Presentation.Infrastructure.Tests.Validation
{
	internal class AtLeastOneEnabledAttributeTests
	{
		private class TestClass
		{
			[AtLeastOneEnabled("Group")]
			public bool First { get; set; }

			[AtLeastOneEnabled("Group")]
			public bool Second { get; set; }

			[AtLeastOneEnabled("Group")]
			public bool Third { get; set; }

			[AtLeastOneEnabled("Group")]
			public int IntPropertyInGroup { get; set; }

			public bool PropertyNotInGroup { get; set; }
		}

		private class TestClassWithTwoGroups
		{
			[AtLeastOneEnabled("Group")]
			public bool First { get; set; }

			[AtLeastOneEnabled("Group")]
			public bool Second { get; set; }

			[AtLeastOneEnabled("AnotherGroup")]
			public bool FirstInAnotherGroup { get; set; }

			[AtLeastOneEnabled("AnotherGroup")]
			public bool SecondInAnotherGroup { get; set; }
		}

		[Test]
		public void WhenNoPropertyIsEnabledValidationFails()
		{
			bool valid = Validate(new TestClass());

			Assert.That(valid, Is.False);
		}

		[Test]
		public void WhenOnePropertyIsEnabledValidationSucceeds()
		{
			bool valid = Validate(new TestClass { Second = true });

			Assert.That(valid, Is.True);
		}

		[Test]
		public void WhenPropertyOutsideOfGroupIsEnabledItIsIgnored()
		{
			bool valid = Validate(new TestClass { PropertyNotInGroup = true });

			Assert.That(valid, Is.False);
		}

		[Test]
		public void WhenMoreThanOnePropertyIsEnabledValidationSucceeds()
		{
			bool valid = Validate(new TestClass { Second = true, Third = true });

			Assert.That(valid, Is.True);
		}

		[Test]
		public void NonBooleablePropertiesAreIgnored()
		{
			bool valid = Validate(new TestClass { IntPropertyInGroup = 123 });

			Assert.That(valid, Is.False);
		}

		[Test]
		public void GroupsAreIndependent()
		{
			bool valid = Validate(new TestClassWithTwoGroups { FirstInAnotherGroup = true });

			Assert.That(valid, Is.False);
		}

		private bool Validate(object instance)
		{
			return Validator.TryValidateObject(instance, new ValidationContext(instance), new Collection<ValidationResult>(),
			                                   validateAllProperties: true);
		}
	}
}
