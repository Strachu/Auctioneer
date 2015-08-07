using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Resources.User.Create;

namespace Auctioneer.Presentation.Models.User
{
	public class UserCreateViewModel
	{
		[Required]
		[StringLength(25, MinimumLength = 4)]
		[Display(Name = "Username", ResourceType = typeof(Lang))]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Lang))]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "ConfirmPassword", ResourceType = typeof(Lang))]
		[Compare("Password", ErrorMessageResourceName = "PasswordsDoNotMatch", ErrorMessageResourceType = typeof(Lang))]
		public string RepeatedPassword { get; set; } 

		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email", ResourceType = typeof(Lang))]
		public string Email { get; set; }

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
	}
}