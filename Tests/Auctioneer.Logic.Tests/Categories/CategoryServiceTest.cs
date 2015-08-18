using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.ValueTypes;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Categories
{
	[TestFixture]
	class CategoryServiceTest
	{
		private ICategoryService mTestedService;

		[SetUp]
		public void SetUp()
		{
			var context = new TestAuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(context);

			mTestedService = new CategoryService(context);
		}

		private void AddTestData(AuctioneerDbContext context)
		{
			context.Categories.Add(new TestCategory
			{
				Id = 1, Name = "Computers", Left = 1, Right = 22,
				SubCategories = new TestCategory[]
				{
					new TestCategory { Id = 2, Name = "Desktop computers", Left = 2, Right = 3 },
					new TestCategory { Id = 3, Name = "Mobile computers",  Left = 4, Right = 5 },
					new TestCategory
					{
						Id = 4, Name = "Components", Left = 6, Right = 21,
						SubCategories = new TestCategory[]
						{
							new TestCategory { Id = 5,  Name = "Hard drives",    Left = 7, Right = 8 },
							new TestCategory { Id = 6,  Name = "Graphics cards", Left = 9, Right = 10 },
							new TestCategory { Id = 7,  Name = "Motherboards",   Left = 11, Right = 12 },
							new TestCategory { Id = 8,  Name = "Processors",     Left = 13, Right = 14 },
							new TestCategory { Id = 9,  Name = "RAM memory",     Left = 15, Right = 16 },
							new TestCategory { Id = 15, Name = "Power supplies", Left = 17, Right = 18 },
							new TestCategory { Id = 16, Name = "Cases",          Left = 19, Right = 20 },
						}
					}
				}
			});

			context.Categories.Add(new TestCategory
			{
				Id = 12, Name = "Software", Left = 23, Right = 32,
				SubCategories = new TestCategory[]
				{
					new TestCategory { Id = 13, Name = "Operating systems", Left = 24, Right = 25 },
					new TestCategory { Id = 14, Name = "Office",            Left = 26, Right = 27 },
					new TestCategory { Id = 10, Name = "Security",          Left = 28, Right = 29 },
					new TestCategory { Id = 11, Name = "Games",             Left = 30, Right = 31 },
				}
			});

			context.SaveChanges();

			context.Users.Add(new TestUser { Id = "1" });

			AddAuctionsToCategory(context, 1,  5);
			AddAuctionsToCategory(context, 2,  3, AuctionStatus.Expired);
			AddAuctionsToCategory(context, 3,  2);
			AddAuctionsToCategory(context, 7,  3);
			AddAuctionsToCategory(context, 8,  7);
			AddAuctionsToCategory(context, 12, 3);
			AddAuctionsToCategory(context, 14, 2, AuctionStatus.Sold);
			AddAuctionsToCategory(context, 11, 10);

			context.SaveChanges();
		}

		private void AddAuctionsToCategory(AuctioneerDbContext context,
		                                   int categoryId,
		                                   int auctionCount,
		                                   AuctionStatus status = AuctionStatus.Active)
		{
			for(int i = 0; i < auctionCount; ++i)
			{
				var auction = new TestAuction
				{
					CategoryId  = categoryId,
					EndDate     = (status != AuctionStatus.Expired) ? DateTime.Now.Add(TimeSpan.FromDays(1))
					                                                : DateTime.Now.Subtract(TimeSpan.FromDays(1)),
					BuyoutPrice = new Money(10, new TestCurrency())
				};

				if(status == AuctionStatus.Sold)
				{
					auction.Offers.Add(new TestBuyOffer { Amount = 10 } );
				}

				context.Auctions.Add(auction);
			}
		}

		[Test]
		public async Task GetAllCategoriesWithHierarchyInfo_ReturnsCorrectHierarchyLevel()
		{
			var categories = (await mTestedService.GetAllCategoriesWithHierarchyLevel()).ToDictionary(x => x.Category.Name);

			Assert.That(categories["Computers"].HierarchyDepth,         Is.EqualTo(0));
			Assert.That(categories["Desktop computers"].HierarchyDepth, Is.EqualTo(1));
			Assert.That(categories["Mobile computers"].HierarchyDepth,  Is.EqualTo(1));
			Assert.That(categories["Components"].HierarchyDepth,        Is.EqualTo(1));
			Assert.That(categories["Hard drives"].HierarchyDepth,       Is.EqualTo(2));
			Assert.That(categories["Graphics cards"].HierarchyDepth,    Is.EqualTo(2));
			Assert.That(categories["Motherboards"].HierarchyDepth,      Is.EqualTo(2));
			Assert.That(categories["Processors"].HierarchyDepth,        Is.EqualTo(2));
			Assert.That(categories["RAM memory"].HierarchyDepth,        Is.EqualTo(2));
			Assert.That(categories["Power supplies"].HierarchyDepth,    Is.EqualTo(2));
			Assert.That(categories["Cases"].HierarchyDepth,             Is.EqualTo(2));
			Assert.That(categories["Software"].HierarchyDepth,          Is.EqualTo(0));
			Assert.That(categories["Operating systems"].HierarchyDepth, Is.EqualTo(1));
			Assert.That(categories["Office"].HierarchyDepth,            Is.EqualTo(1));
			Assert.That(categories["Security"].HierarchyDepth,          Is.EqualTo(1));
			Assert.That(categories["Games"].HierarchyDepth,             Is.EqualTo(1));
		}

		[Test]
		public async Task GetAllCategoriesWithHierarchyInfo_ReturnsEntriesInDepthFirstOrder()
		{
			var categories = await mTestedService.GetAllCategoriesWithHierarchyLevel();

			var returnedCategoryNames = categories.Select(x => x.Category.Name);
			var expectedCategoryNames = new string[]
			{
				"Computers", "Desktop computers", "Mobile computers", "Components", "Hard drives", "Graphics cards",
				"Motherboards", "Processors", "RAM memory", "Power supplies", "Cases", "Software", "Operating systems",
				"Office", "Security", "Games"
			};

			Assert.That(returnedCategoryNames, Is.EqualTo(expectedCategoryNames));
		}

		[Test]
		public async Task GetCategoriesWithoutParentId_ReturnsAllTopLevelCategoriesAndNothingElse()
		{
			var categories = await mTestedService.GetCategoriesAlongWithAuctionCount();

			var returnedCategoryNames = categories.Select(x => x.Category.Name);
			var expectedCategoryNames = new string[] { "Computers", "Software" };

			Assert.That(returnedCategoryNames, Is.EqualTo(expectedCategoryNames));
		}

		[Test]
		public async Task GetCategoriesWithoutParentId_ReturnsAlsoCorrectInfoAboutAuctionCount()
		{
			var categories = (await mTestedService.GetCategoriesAlongWithAuctionCount()).ToDictionary(x => x.Category.Name);

			Assert.That(categories["Computers"].AuctionCount, Is.EqualTo(17));
			Assert.That(categories["Software"].AuctionCount,  Is.EqualTo(13));
		}

		[Test]
		public async Task GetCategories_ReturnsTheCategoriesInAlphabeticalOrder()
		{
			var categories = await mTestedService.GetCategoriesAlongWithAuctionCount(parentCategoryId: 4);

			var returnedSubCategoryNames = categories.Select(x => x.Category.Name);
			var expectedSubCategoryNames = new string[]
			{
				"Cases", "Graphics cards", "Hard drives", "Motherboards", "Power supplies", "Processors", "RAM memory", 
			};

			Assert.That(returnedSubCategoryNames, Is.EqualTo(expectedSubCategoryNames));
		}

		[Test]
		public async Task GetCategories_ReturnsAlsoCategoriesWithOnlyInactiveAuctions()
		{
			var categories = await mTestedService.GetCategoriesAlongWithAuctionCount(parentCategoryId: 1);

			var returnedSubCategoryNames = categories.Select(x => x.Category.Name);
			var expectedSubCategoryNames = new string[] { "Desktop computers", "Mobile computers", "Components" };

			Assert.That(returnedSubCategoryNames, Is.EquivalentTo(expectedSubCategoryNames));
		}

		[Test]
		public async Task GetCategories_ReturnsAlsoCorrectInfoAboutAuctionCountForLeafCategories()
		{
			var categories = (await mTestedService.GetCategoriesAlongWithAuctionCount(parentCategoryId: 12))
			                                      .ToDictionary(x => x.Category.Name);

			Assert.That(categories["Operating systems"].AuctionCount, Is.EqualTo(0));
			Assert.That(categories["Office"].AuctionCount,            Is.EqualTo(0));
			Assert.That(categories["Security"].AuctionCount,          Is.EqualTo(0));
			Assert.That(categories["Games"].AuctionCount,             Is.EqualTo(10));
		}

		[Test]
		public async Task GetCategoryHierarchy_ReturnsAllCategoriesAlongHierarchyInTopBottomOrder()
		{
			var categories = await mTestedService.GetCategoryHierarchy(7);

			var returnedCategoryNames = categories.Select(x => x.Name);
			var expectedCategoryNames = new string[] { "Computers", "Components", "Motherboards" };

			Assert.That(returnedCategoryNames, Is.EqualTo(expectedCategoryNames));
		}
	}
}
