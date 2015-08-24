using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.ValueTypes;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class IQueryableAuctionExtensions
	{
		private IQueryable<Auction> mQueryable;
		private AuctioneerDbContext mDbContext;

		[SetUp]
		public void SetUp()
		{
			mDbContext = new TestAuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(mDbContext);

			mQueryable = mDbContext.Auctions;
		}

		private void AddTestData(AuctioneerDbContext context)
		{
			context.Categories.Add(new TestCategory { Id = 1 });
			context.Users.Add(new TestUser { Id = "1" });
			context.SaveChanges();

			context.Auctions.Add(new TestAuction
			{
				Id           = 1,
				BuyoutPrice  = new Money(10, new TestCurrency()),
				MinimumPrice = new Money( 5, new TestCurrency()),
			});

			context.Auctions.Add(new TestAuction
			{
				Id           = 2,
				MinimumPrice = new Money(1, new TestCurrency()),
				Offers = new Collection<BuyOffer>
				{
					new TestBuyOffer { Amount = 5 },
					new TestBuyOffer { Amount = 8 },
				}
			});

			context.Auctions.Add(new TestAuction
			{
				Id          = 3,
				BuyoutPrice = new Money(6, new TestCurrency()),
			});

			context.Auctions.Add(new TestAuction
			{
				Id           = 4,
				BuyoutPrice  = new Money(50, new TestCurrency()),
				MinimumPrice = new Money(2, new TestCurrency()),
				Offers       = new Collection<BuyOffer>
				{
					new TestBuyOffer { Amount = 3 },
					new TestBuyOffer { Amount = 4 },
				}
			});

			context.SaveChanges();
		}
		
		[Test]
		public void OrderByMinimumPrice_SortsByMinimumOf_MaxOffer_MinBid_BuyoutPrice()
		{
			var auctions = mQueryable.OrderByMinimumPrice();

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { 4, 1, 3, 2 };

			Assert.That(returnedAuctionIds, Is.EqualTo(expectedAuctionIds));
		}
		
		[Test]
		public void OrderByMinimumPriceDescending_SortsByMinimumOf_MaxOffer_MinBid_BuyoutPrice()
		{
			var auctions = mQueryable.OrderByMinimumPriceDescending();

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { 2, 3, 1, 4 };

			Assert.That(returnedAuctionIds, Is.EqualTo(expectedAuctionIds));
		}

		private void InitializeThenByBuyoutPriceData()
		{
			mDbContext.Auctions.RemoveRange(mDbContext.Auctions);

			mDbContext.Auctions.Add(new TestAuction
			{
				Id           = 1,
				BuyoutPrice  = new Money(10, new TestCurrency()),
				MinimumPrice = new Money( 5, new TestCurrency()),
			});
			mDbContext.Auctions.Add(new TestAuction
			{
				Id           = 2,
				BuyoutPrice  = new Money(15, new TestCurrency()),
				MinimumPrice = new Money( 5, new TestCurrency()),
			});
			mDbContext.Auctions.Add(new TestAuction
			{
				Id           = 3,
				BuyoutPrice  = new Money(12, new TestCurrency()),
				MinimumPrice = new Money( 5, new TestCurrency()),
			});

			mDbContext.SaveChanges();
		}

		[Test]
		public void OrderByMinimumPrice_WhenBestOfferIsIdenticalThenSortByBuyoutPrice()
		{
			InitializeThenByBuyoutPriceData();

			var auctions = mQueryable.OrderByMinimumPrice();

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { 1, 3, 2 };

			Assert.That(returnedAuctionIds, Is.EqualTo(expectedAuctionIds));
		}

		[Test]
		public void OrderByMinimumPriceDescending_WhenBestOfferIsIdenticalThenSortByBuyoutPrice()
		{
			InitializeThenByBuyoutPriceData();

			var auctions = mQueryable.OrderByMinimumPriceDescending();

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { 2, 3, 1 };

			Assert.That(returnedAuctionIds, Is.EqualTo(expectedAuctionIds));
		}
	}
}
