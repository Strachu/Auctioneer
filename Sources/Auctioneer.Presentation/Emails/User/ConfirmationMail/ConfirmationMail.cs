using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class ConfirmationMail : Email
	{
		public ConfirmationMail() : base("~/Emails/User/ConfirmationMail/ConfirmationMail.cshtml")
		{
		}

		public string To { get; set; }
		public string UserFirstName { get; set; }
		public string UserId { get; set; }
		public string ConfirmToken { get; set; } 
	}
}