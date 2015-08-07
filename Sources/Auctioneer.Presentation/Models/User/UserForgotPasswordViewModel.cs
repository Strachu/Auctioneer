using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Resources.User.ForgotPassword;

namespace Auctioneer.Presentation.Models.User
{
	public class UserForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email", ResourceType = typeof(Lang))]
		public string Email { get; set; }
	}
}