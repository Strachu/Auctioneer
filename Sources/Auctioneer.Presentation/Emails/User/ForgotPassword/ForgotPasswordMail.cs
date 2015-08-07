using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class ForgotPasswordMail : Email
	{
		public ForgotPasswordMail() : base("~/Emails/User/ForgotPassword/ForgotPasswordMail.cshtml")
		{
		}

		public string UserMail { get; set; }
		public string UserFirstName { get; set; }
		public string PasswordResetToken { get; set; } 
	}
}