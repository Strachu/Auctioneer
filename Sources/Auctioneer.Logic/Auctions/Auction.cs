using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Users;
using Auctioneer.Logic.ValueTypes;

namespace Auctioneer.Logic.Auctions
{
	public class Auction
	{
		public Auction()
		{
			Offers = new List<BuyOffer>();
		}

		public int Id { get; set; }

		public string Title { get; set; }
		public string Description { get; set; }

		public DateTime CreationDate { get; set; }
		public DateTime EndDate { get; set; }

		public virtual Money MinimumPrice { get; set; }
		public virtual Money BuyoutPrice { get; set; }
		
		public bool IsBiddingEnabled
		{
			get { return MinimumPrice != null; }
		}

		public bool IsBuyoutEnabled
		{
			get
			{
				// TODO buyout should become disabled when the best offer is less than 10% lower than buyout price
				return BuyoutPrice != null;
			}
		}

		public decimal MinBidAllowed
		{
			get
			{
				if(!IsBiddingEnabled)
					return 0;

				var bestOffer = BestOffer;
				if(bestOffer == null)
					return MinimumPrice.Amount;

				return Math.Ceiling(bestOffer.Amount * 1.01m);
			}
		}

		public int PhotoCount { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }

		public string SellerId { get; set; }
		public virtual User Seller { get; set; }

		public virtual ICollection<BuyOffer> Offers { get; set; }

		public User Buyer
		{
			get
			{
				if(Status != AuctionStatus.Sold)
					return null;

				var bestOffer = Offers.OrderByDescending(x => x.Amount).FirstOrDefault();
				if(bestOffer == null)
					return null;

				return bestOffer.User;
			}
		}

		public AuctionStatus Status
		{
			get
			{
				if(BuyoutPrice != null && Offers.Any(x => x.Amount >= BuyoutPrice.Amount))
					return AuctionStatus.Sold;

				if(EndDate < DateTime.Now)
					return Offers.Any() ? AuctionStatus.Sold : AuctionStatus.Expired;

				return AuctionStatus.Active;
			}
		}

		public Money BestOffer
		{
			get
			{
				var bestOffer = Offers.OrderByDescending(x => x.Amount).FirstOrDefault();
				if(bestOffer == null)
					return null;

				return new Money(bestOffer.Amount, (MinimumPrice != null) ? MinimumPrice.Currency : BuyoutPrice.Currency);
			}
		}
	}
}
