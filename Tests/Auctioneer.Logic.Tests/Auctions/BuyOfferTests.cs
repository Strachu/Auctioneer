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
	internal class BuyOfferTests
	{
		[Test]
		public void BuyOfferIsNotBuyoutOfferIfItIsLessThanAuctionBuyoutPrice()
		{
			var offer = new BuyOffer
			{
				Auction = new TestAuction { BuyoutPrice = new Money(100.0m, new TestCurrency()) },
				Amount  = 75.0m,
			};

			Assert.That(offer.IsBuyout, Is.False);
		}

		[Test]
		public void BuyOfferIsNotBuyoutOfferWhenBuyoutIsNotEnabled()
		{
			var offer = new BuyOffer
			{
				Auction = new TestAuction(),
				Amount  = 100.0m,
			};

			Assert.That(offer.IsBuyout, Is.False);
		}

		[Test]
		public void BuyOfferIsBuyoutOfferIfItIsEqualToAuctionBuyoutPrice()
		{
			var offer = new BuyOffer
			{
				Auction = new TestAuction { BuyoutPrice = new Money(100.0m, new TestCurrency()) },
				Amount  = 100.0m,
			};

			Assert.That(offer.IsBuyout, Is.True);
		}
	}
}
