using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class AuctionWonMail : Email
	{
		public AuctionWonMail() : base("~/Emails/Auction/Won/AuctionWonMail.cshtml")
		{
		}

		public string UserMail { get; set; }
		public string UserFirstName { get; set; }

		public int    AuctionId { get; set; }
		public string AuctionTitle { get; set; }

		[DataType(DataType.Currency)]
		public decimal AuctionPrice { get; set; }

		public string SellerUserName { get; set; }

		[DataType(DataType.EmailAddress)]
		public string SellerEmail { get; set; }
		public string SellerFullName { get; set; }
	}
}