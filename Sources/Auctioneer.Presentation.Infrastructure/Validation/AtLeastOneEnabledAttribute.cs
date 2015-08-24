using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Auctioneer.Presentation.Infrastructure.Validation
{
	public class AtLeastOneEnabledAttribute : ValidationAttribute
	{
		public AtLeastOneEnabledAttribute(string groupName)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(groupName));

			GroupName = groupName;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var propertiesInGroup = GetPropertiesInTheSameGroup(validationContext);
			if(!propertiesInGroup.Any(x => x.GetValue(validationContext.ObjectInstance).Equals(true)))
				return new ValidationResult(FormatErrorMessage(validationContext.MemberName));

			return ValidationResult.Success;
		}

		private IEnumerable<PropertyInfo> GetPropertiesInTheSameGroup(ValidationContext validationContext)
		{
			return validationContext.ObjectType.GetProperties().Where(prop =>
			{
				var attributes = prop.GetCustomAttributes(typeof(AtLeastOneEnabledAttribute), inherit: true);

				return attributes.Cast<AtLeastOneEnabledAttribute>().Any(x => x.GroupName == this.GroupName);
			});
		}

		public string GroupName { get; private set;}

		public override bool RequiresValidationContext
		{
			get { return true; }
		}
	}
}