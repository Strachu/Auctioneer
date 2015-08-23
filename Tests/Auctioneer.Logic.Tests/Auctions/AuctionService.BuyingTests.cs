using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Tests.TestUtils;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.Users;
using Auctioneer.Logic.ValueTypes;

using FakeItEasy;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class AuctionServiceBuyingTests
	{
		private IAuctionService     mTestedService;
		private AuctioneerDbContext mDbContext;
		private IUserNotifier       mUserNotifierMock;

		[SetUp]
		public void SetUp()
		{
			mDbContext = new TestAuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			mDbContext.Categories.Add(new TestCategory { Id = 1 });
			mDbContext.Users.Add(new TestUser { Id = "1" });
			mDbContext.Users.Add(new TestUser { Id = "2" });
			mDbContext.Users.Add(new TestUser { Id = "3" });
			mDbContext.SaveChanges();

			mUserNotifierMock = A.Fake<IUserNotifier>();
			var userService   = new UserService(mDbContext, mUserNotifierMock);
			mTestedService    = new AuctionService(mDbContext, mUserNotifierMock, userService, "Ignored", "Ignored");
		}

		[Test]
		public void UserCannotBuyHisOwnAuctions()
		{
			var auction = new TestAuction
			{
				SellerId = "1",
				EndDate  = DateTime.Now.Add(TimeSpan.FromDays(2)),
			};

			var result = mTestedService.CanBeBought(auction, buyerId: "1");

			Assert.That(result, Is.False);
		}

		[Test]
		public void WhenAuctionIsInactive_ItCannotBeBought()
		{
			var auction = new TestAuction
			{
				SellerId = "1",
				EndDate  = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
			};

			var result = mTestedService.CanBeBought(auction, buyerId: "2");

			Assert.That(result, Is.False);
		}

		[Test]
		public void WhenAuctionIsAlreadySold_ItCannotBeBoughtAgain()
		{
			var auction = new TestAuction
			{
				SellerId    = "1",
				EndDate     = DateTime.Now.Add(TimeSpan.FromDays(2)),
				BuyoutPrice = new Money(100, new TestCurrency()),
				Offers      = new Collection<BuyOffer> { new TestBuyOffer { UserId = "3", Amount = 100 } }
			};

			var result = mTestedService.CanBeBought(auction, buyerId: "2");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task WhenTryingToBuyoutAnAuctionWithoutBuyoutEnabled_ValidationErrorIsReturned()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId = "2",
				EndDate  = DateTime.Now.Add(TimeSpan.FromDays(2)),
			});

			var errors = new FakeErrorNotifier();

			await mTestedService.Buyout(auction.Id, buyerId: "1", errors: errors);

			Assert.That(errors.IsInErrorState, Is.True);
		}

		[Test]
		public async Task BuyingAuctionAddsOfferWithBuyoutPrice()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId    = "2",
				EndDate     = DateTime.Now.Add(TimeSpan.FromDays(2)),
				BuyoutPrice = new Money(10, new TestCurrency())
			});

			await mTestedService.Buyout(auction.Id, buyerId: "1", errors: new FakeErrorNotifier());

			var boughtAction = await mTestedService.GetById(auction.Id);

			Assert.That(boughtAction.Status, Is.EqualTo(AuctionStatus.Sold));
			Assert.That(boughtAction.Offers.Single().UserId, Is.EqualTo("1"));
			Assert.That(boughtAction.Offers.Single().Amount, Is.EqualTo(10));
		}

		[Test]
		public async Task SellerIsNotifiedAfterHisAuctionHasBeenSold()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId    = "2",
				EndDate     = DateTime.Now.Add(TimeSpan.FromDays(2)),
				BuyoutPrice = new Money(10, new TestCurrency())
			});

			await mTestedService.Buyout(auction.Id, buyerId: "1", errors: new FakeErrorNotifier());

			A.CallTo(() => mUserNotifierMock.NotifyAuctionSold(A<User>.That.Matches(x => x.Id == "2"), A<Auction>._))
			 .MustHaveHappened();
		}

		[Test]
		public async Task BuyerIsNotifiedWhenHeBuysAnAuction()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				EndDate     = DateTime.Now.Add(TimeSpan.FromDays(2)),
				BuyoutPrice = new Money(10, new TestCurrency())
			});

			await mTestedService.Buyout(auction.Id, buyerId: "2", errors: new FakeErrorNotifier());

			A.CallTo(() => mUserNotifierMock.NotifyAuctionWon(A<User>.That.Matches(x => x.Id == "2"), A<Auction>._))
			 .MustHaveHappened();
		}

		[Test]
		public async Task WhenTryingToBidOnAuctionWithoutBiddingEnabled_ValidationErrorIsReturned()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId = "2",
				EndDate  = DateTime.Now.Add(TimeSpan.FromDays(2)),
			});

			var errors = new FakeErrorNotifier();

			await mTestedService.Bid(auction.Id, buyerId: "1", bidAmount: 100, errors: errors);

			Assert.That(errors.IsInErrorState, Is.True);
		}

		[Test]
		public async Task WhenBidAmountIsLowerThanMinimumAllowedBid_ValidationErrorIsReturned()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId     = "2",
				EndDate      = DateTime.Now.Add(TimeSpan.FromDays(2)),
				MinimumPrice = new Money(100.0m, new TestCurrency()),
				Offers       = new BuyOffer[] { new TestBuyOffer { Amount = 150.0m } }
			});

			var errors = new FakeErrorNotifier();

			await mTestedService.Bid(auction.Id, buyerId: "1", bidAmount: 150.0m, errors: errors);

			Assert.That(errors.IsInErrorState, Is.True);
		}

		[Test]
		public async Task WhenAuctionCannotBeBought_BidReturnsValidationError()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId     = "1",
				EndDate      = DateTime.Now.Add(TimeSpan.FromDays(2)),
				MinimumPrice = new Money(100.0m, new TestCurrency()),
			});

			var errors = new FakeErrorNotifier();

			await mTestedService.Bid(auction.Id, buyerId: "1", bidAmount: 110.0m, errors: errors);

			Assert.That(errors.IsInErrorState, Is.True);
		}

		[Test]
		public async Task BidAddsNewOfferWhenValidationPasses()
		{
			var auction = AddAuctionToDatabase(new TestAuction
			{
				SellerId     = "2",
				EndDate      = DateTime.Now.Add(TimeSpan.FromDays(2)),
				MinimumPrice = new Money(100.0m, new TestCurrency()),
				Offers       = new Collection<BuyOffer> { new TestBuyOffer { Amount = 150.0m } }
			});

			await mTestedService.Bid(auction.Id, buyerId: "1", bidAmount: 200.0m, errors: new FakeErrorNotifier());

			var bidAuction = await mTestedService.GetById(auction.Id);
			var newOffer   = bidAuction.Offers.Single(x => x.Amount == 200.0m);

			Assert.That(newOffer.AuctionId, Is.EqualTo(auction.Id));
			Assert.That(newOffer.UserId,    Is.EqualTo("1"));
			Assert.That(newOffer.Amount,    Is.EqualTo(200.0m));
			Assert.That(newOffer.Date,      Is.EqualTo(DateTime.Now).Within(1).Minutes);
		}

		private Auction AddAuctionToDatabase(Auction auction)
		{
			mDbContext.Auctions.Add(auction);
			mDbContext.SaveChanges();

			return auction;
		}
	}
}
