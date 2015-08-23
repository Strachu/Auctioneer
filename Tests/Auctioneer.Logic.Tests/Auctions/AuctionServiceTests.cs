using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Currencies;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.Users;
using Auctioneer.Logic.ValueTypes;

using FakeItEasy;

using Microsoft.AspNet.Identity.EntityFramework;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class AuctionServiceTests
	{
		private IAuctionService     mTestedService;
		private AuctioneerDbContext mDbContext;
		private IUserNotifier       mUserNotifierMock;

		[SetUp]
		public void SetUp()
		{
			mDbContext = new TestAuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(mDbContext);

			mUserNotifierMock = A.Fake<IUserNotifier>();
			var userService   = new UserService(mDbContext, mUserNotifierMock);
			mTestedService    = new AuctionService(mDbContext, mUserNotifierMock, userService, "Ignored", "Ignored");
		}

		private void AddTestData(AuctioneerDbContext context)
		{
			context.Categories.AddRange(new TestCategory[]
			{
				new TestCategory
				{
					Id = 1, Left = 1, Right = 10,
					SubCategories = new TestCategory[]
					{
						new TestCategory { Id = 2, Left = 2, Right = 3 },		
						new TestCategory
						{
							Id = 3, Left = 4, Right = 9,						
							SubCategories = new TestCategory[]
							{
								new TestCategory { Id = 4, Left = 5, Right = 6 },		
								new TestCategory { Id = 5, Left = 7, Right = 8 },					
							}	
						},				

					}
				},
				new TestCategory { Id = 6, Left = 11, Right = 12 },
			});

			context.Users.Add(new TestUser { Id = "1" });
			context.Users.Add(new TestUser { Id = "2" });
			context.Users.Add(new TestUser { Id = "3" });
			context.Roles.Add(new IdentityRole { Name = "Admin" });

			context.SaveChanges();

			context.Users.Find("3").Roles.Add(new IdentityUserRole { RoleId = context.Roles.Single().Id, UserId = "3" });

			// TODO the tests are hard to update due to dependencies between test data, how to solve this?
			// The best would be independent data tailored just for single test, but there is a lot of data to setup
			// which will make the class with tests very long.

			context.Auctions.Add(new TestAuction { Id = 1,  Title = "1",  CategoryId = 2, CreationDate = new DateTime(2015, 3, 12),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(2)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 2,  Title = "2",  CategoryId = 6, CreationDate = new DateTime(2013, 5, 25),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 3,  Title = "3",  CategoryId = 6, CreationDate = new DateTime(2013, 6, 11),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)), SellerId = "2" });

			context.Auctions.Add(new TestAuction { Id = 4,  Title = "4",  CategoryId = 2, CreationDate = new DateTime(2014, 9, 16),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "1",
																BuyoutPrice = new Money(100, new Currency("$", CurrencySymbolPosition.BeforeAmount)),
																Offers = new Collection<BuyOffer> { new TestBuyOffer { UserId = "2", Amount = 100 } } });

			context.Auctions.Add(new TestAuction { Id = 5,  Title = "5",  CategoryId = 2, CreationDate = new DateTime(2013, 9, 5),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 6,  Title = "6",  CategoryId = 2, CreationDate = new DateTime(2012, 1, 16),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 7,  Title = "7",  CategoryId = 2, CreationDate = new DateTime(2014, 12, 22),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(10)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 8,  Title = "8",  CategoryId = 2, CreationDate = new DateTime(2013, 1, 12),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromMinutes(1)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 9,  Title = "9",  CategoryId = 3, CreationDate = new DateTime(2015, 2, 1),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "2" });

			context.Auctions.Add(new TestAuction { Id = 10, Title = "10", CategoryId = 5, CreationDate = new DateTime(2014, 4, 30),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Id = 11, Title = "11",  CategoryId = 3, CreationDate = new DateTime(2013, 2, 1),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(1)), SellerId = "2" });

			context.Auctions.Add(new TestAuction { Id = 12, Title = "12",  CategoryId = 3, CreationDate = new DateTime(2013, 2, 1),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(1)), SellerId = "1" });
			context.SaveChanges();
		}

		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsOnlyAuctionsFromSpecifiedCategory()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(categoryId: 6,
			                                                                sortBy: AuctionSortOrder.TitleAscending,
			                                                                pageIndex: 1,
			                                                                auctionsPerPage: 100);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "2", "3" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}

		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsCategoriesCorrectlyPaged()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(categoryId: 1,
			                                                                sortBy: AuctionSortOrder.TitleAscending,
			                                                                pageIndex: 2,
			                                                                auctionsPerPage: 2);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "7", "8" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetActiveAuctionsInCategory_DoesNotReturnExpiredNorAlreadyBoughtAuctions()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(categoryId: 2,
			                                                                sortBy: AuctionSortOrder.TitleAscending,
			                                                                pageIndex: 1,
			                                                                auctionsPerPage: 100);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "5", "7", "8" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsAlsoAuctionsFromSubcategories()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(categoryId: 1,
			                                                                sortBy: AuctionSortOrder.TitleAscending,
			                                                                pageIndex: 1,
			                                                                auctionsPerPage: 100);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "5", "7", "8", "9", "10" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetRecentAuctions_ReturnsSpecifiedNumberOfAuctionsSortedByTheirCreationDate()
		{
			var auctions = await mTestedService.GetRecentAuctions(3);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "9", "7", "10" };

			Assert.That(returnedAuctionTitles, Is.EqualTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetAuctionsByUser_ReturnsOnlyAuctionsAddedBySpecifiedUser()
		{
			var auctions = await mTestedService.GetAuctionsByUser(userId: "2", createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "3", "9", "11" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task WhenUserIsNotTheCreaterOfTheAuction_CanBeRemovedByUser_ReturnsFalse()
		{
			var auction = await mTestedService.GetById(5);
			var result  = await mTestedService.CanBeRemoved(auction, userId: "2");

			Assert.That(result, Is.False);
		}
		
		[Test]
		public async Task AdminCanRemoveAuctionsCreatedByOtherUsers()
		{
			var auction = await mTestedService.GetById(5);
			var result  = await mTestedService.CanBeRemoved(auction, userId: "3");

			Assert.That(result, Is.True);
		}
		
		[Test]
		public async Task RemovalOfExpiredAuctionsIsNotAllowed()
		{
			var auction = await mTestedService.GetById(6);
			var result  = await mTestedService.CanBeRemoved(auction, userId: "1");

			Assert.That(result, Is.False);
		}
		
		[Test]
		public async Task RemovalOfSoldAuctionsIsNotAllowed()
		{
			var auction = await mTestedService.GetById(4);
			var result  = await mTestedService.CanBeRemoved(auction, userId: "1");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task RemovalOfAuctionsAfterSomeoneRaisedAnOfferIsNotAllowed()
		{
			var auction = new TestAuction
			{
				SellerId = "2",
				EndDate  = DateTime.Now.Add(TimeSpan.FromDays(2)),
				Offers   = new Collection<BuyOffer> { new TestBuyOffer() }
			};

			var result = await mTestedService.CanBeRemoved(auction, userId: "2");

			Assert.That(result, Is.False);
		}
	}
}
