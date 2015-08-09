using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class AuctionSoldMail : Email
	{
		public AuctionSoldMail() : base("~/Emails/Auction/Sold/AuctionSoldMail.cshtml")
		{
		}

		public string UserMail { get; set; }
		public string UserFirstName { get; set; }

		public int    AuctionId { get; set; }
		public string AuctionTitle { get; set; }

		[DataType(DataType.Currency)]
		public decimal AuctionPrice { get; set; }

		public string BuyerUserName { get; set; }

		[DataType(DataType.EmailAddress)]
		public string BuyerEmail { get; set; }
		public string BuyerFullName { get; set; }

		[DataType(DataType.MultilineText)]
		public string BuyerShippingAddress { get; set; }
	}
}