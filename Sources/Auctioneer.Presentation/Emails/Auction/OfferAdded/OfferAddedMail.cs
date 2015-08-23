using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Auctioneer.Logic.ValueTypes;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class OfferAddedMail : Email
	{
		public OfferAddedMail() : base("~/Emails/Auction/OfferAdded/OfferAddedMail.cshtml")
		{
		}

		public string UserMail { get; set; }
		public string UserFirstName { get; set; }

		public int    AuctionId { get; set; }
		public string AuctionTitle { get; set; }

		[DataType(DataType.Currency)]
		public Money OfferMoney { get; set;}
	}
}