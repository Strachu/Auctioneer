using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class OutbidMail : Email
	{
		public OutbidMail() : base("~/Emails/Auction/Outbid/OutbidMail.cshtml")
		{
		}

		public string UserMail { get; set; }
		public string UserFirstName { get; set; }

		public int    AuctionId { get; set; }
		public string AuctionTitle { get; set; }
	}
}