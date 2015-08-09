using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class AuctionServiceTests
	{
		private IAuctionService mTestedService;

		[SetUp]
		public void SetUp()
		{
			var context = new TestAuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(context);

			mTestedService = new AuctionService(context, "Ignored", "Ignored");
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
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "1", BuyerId = "2" });

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
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(1)), SellerId = "1",
			                                       BuyerId = "2" });
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
		public async Task WhenOnlyExpiredStatusFlagHasBeenSpecified_GetAuctionsByUser_DoesNotReturnActiveNorSoldAuctions()
		{
			var auctions = await mTestedService.GetAuctionsByUser(userId: "1", statusFilter: AuctionStatusFilter.Expired,
			                                                      createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "1", "6" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task WhenOnlyActiveStatusFlagHasBeenSpecified_GetAuctionsByUser_DoesNotReturnInactiveNorSoldAuctions()
		{
			var auctions = await mTestedService.GetAuctionsByUser(userId: "1", statusFilter: AuctionStatusFilter.Active,
			                                                      createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "2", "5", "7", "8", "10" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task WhenOnlySoldStatusFlagHasBeenSpecified_GetAuctionsByUser_DoesNotReturnNotSoldAuctions()
		{
			var auctions = await mTestedService.GetAuctionsByUser(userId: "1", statusFilter: AuctionStatusFilter.Sold,
			                                                      createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "4", "12" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task WhenBothExpiredAndSoldStatusFlagsBeenSpecified_GetAuctionsByUser_ReturnsOnlyInactiveAuctions()
		{
			var statusFilters = AuctionStatusFilter.Expired | AuctionStatusFilter.Sold;
			var auctions      = await mTestedService.GetAuctionsByUser(userId: "1", statusFilter: statusFilters,
			                                                           createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "1", "4", "6", "12" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public void WhenUserTriesToRemoveAuctionOfAnotherUser_AnExceptionIsThrown()
		{
			Assert.Throws<LogicException>(async () =>
			{
				await mTestedService.RemoveAuctions(removingUserId: "2", ids: new int[] { 4, 5 });
			});
		}
		
		[Test]
		public void DoNotAllowRemovalOfExpiredAuctions()
		{
			Assert.Throws<LogicException>(async () =>
			{
				await mTestedService.RemoveAuctions(removingUserId: "1", ids: new int[] { 7, 8, 6 });
			});
		}
		
		[Test]
		public void DoNotAllowRemovalOfSoldAuctions()
		{
			Assert.Throws<LogicException>(async () =>
			{
				await mTestedService.RemoveAuctions(removingUserId: "1", ids: new int[] { 7, 4, 8 });
			});
		}

		[Test]
		public async Task UserCannotBuyHisOwnAuctions()
		{
			var auction = await mTestedService.GetById(2);
			var result  = mTestedService.CanBeBought(auction, buyerId: "1");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task WhenAuctionIsInactive_ItCannotBeBought()
		{
			var auction = await mTestedService.GetById(6);
			var result  = mTestedService.CanBeBought(auction, buyerId: "2");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task WhenAuctionIsAlreadySold_ItCannotBeBoughtAgain()
		{
			var auction = await mTestedService.GetById(4);
			var result  = mTestedService.CanBeBought(auction, buyerId: "2");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task BuyingAuctionSetsTheBuyerId()
		{
			await mTestedService.Buy(3, buyerId: "1");

			var boughtAction = await mTestedService.GetById(3);

			Assert.That(boughtAction.BuyerId, Is.EqualTo("1"));
		}
	}
}
