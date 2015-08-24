using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.ValueTypes;
using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class AuctionTests
	{
		[Test]
		public void WhenNoOfferHasBeenRaised_MinBidAllowedIsEqualToMinimumPrice()
		{
			var auction = new Auction { MinimumPrice = new Money(50.0m, new TestCurrency()) };

			Assert.That(auction.MinBidAllowed, Is.EqualTo(auction.MinimumPrice.Amount));
		}

		[Test]
		public void WhenOfferHasAlreadyBeenRaised_MinBidAllowedIsOnePercentHigherThanHighestOfferAndRoundedToInteger()
		{
			var auction = new Auction
			{
				MinimumPrice = new Money(50.0m, new TestCurrency()),
				Offers       = new BuyOffer[]
				{
					new TestBuyOffer { Amount = 125.0m },
					new TestBuyOffer { Amount =  75.0m }
				}
			};

			Assert.That(auction.MinBidAllowed, Is.EqualTo(127.0m));
		}
	}
}
