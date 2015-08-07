using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Compare = System.ComponentModel.DataAnnotations.CompareAttribute;
using Lang    = Auctioneer.Presentation.Resources.User.ResetPassword;

namespace Auctioneer.Presentation.Models.User
{
	public class UserResetPasswordViewModel
	{
		[Required]
		[HiddenInput]
		public string ResetToken { get; set; }

		[Required]
		[Display(Name = "Username", ResourceType = typeof(Lang))]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "NewPassword", ResourceType = typeof(Lang))]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "ConfirmNewPassword", ResourceType = typeof(Lang))]
		[Compare("NewPassword", ErrorMessageResourceName = "PasswordsDoNotMatch", ErrorMessageResourceType = typeof(Lang))]
		public string NewPasswordRepeated { get; set; } 
	}
}