using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Users;
using Auctioneer.Logic.ValueTypes;

namespace Auctioneer.Logic.Auctions
{
	public class BuyOffer
	{
		public int Id { get; set; }

		public int AuctionId { get; set; }
		public virtual Auction Auction { get; set; }

		public string UserId { get; set; }
		public virtual User User { get; set; }

		public DateTime Date { get; set; }
		public decimal Amount { get; set; }

		public bool IsBuyout
		{
			get { return Auction != null && Auction.IsBuyoutEnabled && Amount >= Auction.BuyoutPrice.Amount; }
		}

		public Money Money
		{
			get { return new Money(Amount, IsBuyout ? Auction.BuyoutPrice.Currency : Auction.MinimumPrice.Currency); }
		}
	}
}
