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

			context.Auctions.Add(new TestAuction { Title = "1",  CategoryId = 2, CreationDate = new DateTime(2015, 3, 12),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(2)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "2",  CategoryId = 6, CreationDate = new DateTime(2013, 5, 25),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "3",  CategoryId = 6, CreationDate = new DateTime(2013, 6, 11),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)), SellerId = "2" });

			context.Auctions.Add(new TestAuction { Title = "4",  CategoryId = 2, CreationDate = new DateTime(2014, 9, 16),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "5",  CategoryId = 2, CreationDate = new DateTime(2013, 9, 5),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "6",  CategoryId = 2, CreationDate = new DateTime(2012, 1, 16),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "7",  CategoryId = 2, CreationDate = new DateTime(2014, 12, 22),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(10)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "8",  CategoryId = 2, CreationDate = new DateTime(2013, 1, 12),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromMinutes(1)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "9",  CategoryId = 3, CreationDate = new DateTime(2015, 2, 1),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "2" });

			context.Auctions.Add(new TestAuction { Title = "10", CategoryId = 5, CreationDate = new DateTime(2014, 4, 30),
			                                       EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)), SellerId = "1" });

			context.Auctions.Add(new TestAuction { Title = "11",  CategoryId = 3, CreationDate = new DateTime(2013, 2, 1),
			                                       EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(1)), SellerId = "2" });
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
			var expectedAuctionTitles = new string[] { "5", "7" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsOnlyAuctionsWhoseEndDateIsGreateThanCurrentDate()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(categoryId: 2,
			                                                                sortBy: AuctionSortOrder.TitleAscending,
			                                                                pageIndex: 1,
			                                                                auctionsPerPage: 100);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "4", "5", "7", "8" };

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
			var expectedAuctionTitles = new string[] { "4", "5", "7", "8", "9", "10" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetRecentAuctions_ReturnsSpecifiedNumberOfAuctionsSortedByTheirCreationDate()
		{
			var auctions = await mTestedService.GetRecentAuctions(3);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "1", "9", "7" };

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
		public async Task WhenOnlyExpiredStatusFlagHasBeenSpecified_GetAuctionsByUser_DoesNotReturnActiveAuctions()
		{
			var auctions = await mTestedService.GetAuctionsByUser(userId: "1", statusFilter: AuctionStatusFilter.Expired,
			                                                      createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "1", "6" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task WhenOnlyActiveStatusFlagHasBeenSpecified_GetAuctionsByUser_DoesNotReturnInactiveAuctions()
		{
			var auctions = await mTestedService.GetAuctionsByUser(userId: "1", statusFilter: AuctionStatusFilter.Active,
			                                                      createdIn: TimeSpan.FromDays(9999));

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "2", "4", "5", "7", "8", "10" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
	}
}
