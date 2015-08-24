using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Currencies;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.ValueTypes;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class AuctionStatusDeterminingTests
	{
		private IQueryable<Auction> mQueryable;
		private Auction             mExpiredAuction;
		private Auction             mAuctionSoldByBidding;
		private Auction             mAuctionSoldByBuyout;
		private Auction             mActiveActionWithOffers;
		private Auction             mActiveActionWithoutOffers;

		[SetUp]
		public void SetUp()
		{
			var context = new TestAuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(context);

			mQueryable = context.Auctions;
		}

		private void AddTestData(AuctioneerDbContext context)
		{
			context.Categories.Add(new TestCategory { Id = 1 });
			context.Users.Add(new TestUser { Id = "1" });
			context.SaveChanges();

			mExpiredAuction = new TestAuction
			{
				EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(2))
			};

			mAuctionSoldByBidding = new TestAuction
			{
				EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
				Offers  = new BuyOffer[]
				{
					new TestBuyOffer { Amount = 10 }
				}
			};

			mAuctionSoldByBuyout = new TestAuction
			{
				BuyoutPrice = new Money(100, new Currency("$", CurrencySymbolPosition.BeforeAmount)),
				EndDate     = DateTime.Now.AddDays(3),
				Offers      = new BuyOffer[]
				{
					new TestBuyOffer { Amount = 50 },
					new TestBuyOffer { Amount = 100 }
				}
			};

			mActiveActionWithoutOffers = new TestAuction
			{
				EndDate = DateTime.Now.AddDays(3)
			};

			mActiveActionWithOffers = new TestAuction
			{
				EndDate = DateTime.Now.AddDays(3),
				Offers  = new BuyOffer[]
				{
					new TestBuyOffer { Amount = 50 },
					new TestBuyOffer { Amount = 30 }
				}
			};

			context.Auctions.Add(mExpiredAuction);
			context.Auctions.Add(mAuctionSoldByBidding);
			context.Auctions.Add(mAuctionSoldByBuyout);
			context.Auctions.Add(mActiveActionWithOffers);
			context.Auctions.Add(mActiveActionWithoutOffers);
			context.SaveChanges();
		}

		[Test]
		public void WhenNoBuyOfferHasBeenRaisedAndAuctionDidNotExpireYet_AuctionIsInActiveState()
		{
			Assert.That(mActiveActionWithoutOffers.Status, Is.EqualTo(AuctionStatus.Active));
		}

		[Test]
		public void WhenBuyOfferHasBeenRaisedAndAuctionDidNotExpireYet_AuctionIsStillInActiveState()
		{
			Assert.That(mActiveActionWithOffers.Status, Is.EqualTo(AuctionStatus.Active));
		}

		[Test]
		public void WhenBuyOfferWithAmountEqualToBuyoutPriceWasRaisedAndAuctionDidNotExpireYet_AuctionIsInSoldState()
		{
			Assert.That(mAuctionSoldByBuyout.Status, Is.EqualTo(AuctionStatus.Sold));
		}

		[Test]
		public void WhenAuctionExpiredWithoutAnyOffer_AuctionIsInExpiredState()
		{
			Assert.That(mExpiredAuction.Status, Is.EqualTo(AuctionStatus.Expired));
		}

		[Test]
		public void WhenAuctionExpiredAndThereWasAnOffer_AuctionIsInSoldState()
		{
			Assert.That(mAuctionSoldByBidding.Status, Is.EqualTo(AuctionStatus.Sold));
		}

		[Test]
		public void WhenOnlyExpiredStatusFlagHasBeenSpecified_FilterDoesNotReturnActiveNorSoldAuctions()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.Expired);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mExpiredAuction.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
		
		[Test]
		public void WhenOnlyActiveStatusFlagHasBeenSpecified_FilterDoesNotReturnInactiveNorSoldAuctions()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.Active);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mActiveActionWithOffers.Id, mActiveActionWithoutOffers.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
		
		[Test]
		public void WhenOnlySoldStatusFlagHasBeenSpecified_FilterDoesNotReturnNotSoldAuctions()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.Sold);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mAuctionSoldByBidding.Id, mAuctionSoldByBuyout.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
		
		[Test]
		public void WhenBothExpiredAndSoldStatusFlagsBeenSpecified_FilterReturnsOnlyInactiveAuctions()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.Expired | AuctionStatusFilter.Sold);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mExpiredAuction.Id, mAuctionSoldByBidding.Id, mAuctionSoldByBuyout.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
		
		[Test]
		public void WhenBothExpiredAndActiveStatusFlagsBeenSpecified_FilterDoesNotReturnSoldAuctions()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.Expired | AuctionStatusFilter.Active);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mExpiredAuction.Id, mActiveActionWithoutOffers.Id, mActiveActionWithOffers.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
		
		[Test]
		public void WhenBothActiveAndSoldStatusFlagsBeenSpecified_FilterDoesNotReturnExpiredAuctions()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.Active | AuctionStatusFilter.Sold);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mActiveActionWithoutOffers.Id, mActiveActionWithOffers.Id,
			                                     mAuctionSoldByBidding.Id, mAuctionSoldByBuyout.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
		
		[Test]
		public void WhenAllFlagsHasBeenSpecified_FilterReturnsEveryAuction()
		{
			var auctions = mQueryable.Where(AuctionStatusFilter.All);

			var returnedAuctionIds = auctions.Select(x => x.Id);
			var expectedAuctionIds = new int[] { mExpiredAuction.Id,
			                                     mActiveActionWithoutOffers.Id, mActiveActionWithOffers.Id,
			                                     mAuctionSoldByBidding.Id, mAuctionSoldByBuyout.Id };

			Assert.That(returnedAuctionIds, Is.EquivalentTo(expectedAuctionIds));
		}
	}
}
