using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class AuctionExpiredMail : Email
	{
		public AuctionExpiredMail() : base("~/Emails/Auction/Expired/AuctionExpiredMail.cshtml")
		{
		}

		public string UserMail { get; set; }
		public string UserFirstName { get; set; }

		public int    AuctionId { get; set; }
		public string AuctionTitle { get; set; }
	}
}