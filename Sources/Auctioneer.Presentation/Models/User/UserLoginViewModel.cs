using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lang = Auctioneer.Resources.User.Login;

namespace Auctioneer.Presentation.Models.User
{
	public class UserLoginViewModel
	{
		[Required]
		[Display(Name = "Username", ResourceType = typeof(Lang))]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Lang))]
		public string Password { get; set; }

		[Display(Name = "RememberMe", ResourceType = typeof(Lang))]
		public bool RememberMe { get; set; }

		[HiddenInput]
		public string ReturnUrl { get; set; }
	}
}