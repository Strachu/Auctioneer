using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Presentation.Resources.Account.Edit;

namespace Auctioneer.Presentation.Models.Account
{
	public class AccountEditViewModel
	{
		public LoginDetailsViewModel LoginDetails { get; set; }
		public PersonalInfoViewModel PersonalInfo { get; set; } 

		public class LoginDetailsViewModel
		{
			[Required]
			[EmailAddress]
			[DataType(DataType.EmailAddress)]
			[Display(Name = "Email", ResourceType = typeof(Lang))]
			public string Email { get; set; }

			[Required]
			[DataType(DataType.Password)]
			[Display(Name = "CurrentPassword", ResourceType = typeof(Lang))]
			public string CurrentPassword { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "NewPassword", ResourceType = typeof(Lang))]
			public string NewPassword { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "ConfirmNewPassword", ResourceType = typeof(Lang))]
			[Compare("NewPassword", ErrorMessageResourceName = "PasswordsDoNotMatch", ErrorMessageResourceType = typeof(Lang))]
			public string NewPasswordRepeated { get; set; } 

			public bool ShowValidationSummary { get; set; }

			public LoginDetailsViewModel WithValidationSummary()
			{
				ShowValidationSummary = true;
				return this;
			}
		}

		public class PersonalInfoViewModel
		{
			[Required]
			[Display(Name = "FirstName", ResourceType = typeof(Lang))]
			public string FirstName { get; set; }

			[Required]
			[Display(Name = "LastName", ResourceType = typeof(Lang))]
			public string LastName { get; set; }

			[Required]
			[DataType(DataType.MultilineText)]
			[Display(Name = "Address", ResourceType = typeof(Lang))]
			public string Address { get; set; }

			public bool ShowValidationSummary { get; set; }

			public PersonalInfoViewModel WithValidationSummary()
			{
				ShowValidationSummary = true;
				return this;
			}
		}
	}
}