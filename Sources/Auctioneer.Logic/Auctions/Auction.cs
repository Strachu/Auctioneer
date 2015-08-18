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

		public virtual Money MinBid { get; set; }
		public virtual Money BuyoutPrice { get; set; }

		public int PhotoCount { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }

		public string SellerId { get; set; }
		public virtual User Seller { get; set; }

		public virtual ICollection<BuyOffer> Offers { get; set; }

		// TODO Temporary
		public Money Price
		{
			get
			{
				return BuyoutPrice ?? MinBid;
			}
		}

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
	}
}
