using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Categories
{
	[TestFixture]
	class CategoryServiceTest
	{
		private CategoryService mTestedService;

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
							new TestCategory { Id = 10, Name = "Power supplies", Left = 17, Right = 18 },
							new TestCategory { Id = 11, Name = "Cases",          Left = 19, Right = 20 },
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
					new TestCategory { Id = 15, Name = "Security",          Left = 28, Right = 29 },
					new TestCategory { Id = 16, Name = "Games",             Left = 30, Right = 31 },
				}
			});

			context.SaveChanges();

			AddAuctionsToCategory(context, 1,  5);
			AddAuctionsToCategory(context, 2,  3, active: false);
			AddAuctionsToCategory(context, 3,  2);
			AddAuctionsToCategory(context, 7,  3);
			AddAuctionsToCategory(context, 8,  7);
			AddAuctionsToCategory(context, 12, 3);
			AddAuctionsToCategory(context, 14, 2);
			AddAuctionsToCategory(context, 16, 10);

			context.SaveChanges();
		}

		private void AddAuctionsToCategory(AuctioneerDbContext context, int categoryId, int auctionCount, bool active = true)
		{
			for(int i = 0; i < auctionCount; ++i)
			{
				context.Auctions.Add(new TestAuction
				{
					CategoryId = categoryId,
					EndDate    = active ? DateTime.Now.Add(TimeSpan.FromDays(1)) : DateTime.Now.Subtract(TimeSpan.FromDays(1))
				});
			}
		}

		[Test]
		public async Task GetTopLevelCategories_ReturnsAllTopLevelCategoriesAndNothingElse()
		{
			var categories = await mTestedService.GetTopLevelCategories();

			var returnedCategoryNames = categories.Select(x => x.Name);
			var expectedCategoryNames = new string[] { "Computers", "Software" };

			Assert.That(returnedCategoryNames, Is.EquivalentTo(expectedCategoryNames));
		}

		[Test]
		public async Task GetTopLevelCategories_ReturnsAlsoCorrectInfoAboutAuctionCount()
		{
			var categories = await mTestedService.GetTopLevelCategories();

			var returnedAuctionCount = categories.Select(x => x.AuctionCount);
			var expectedAuctionCount = new int[] { 17, 15 };

			Assert.That(returnedAuctionCount, Is.EqualTo(expectedAuctionCount));
		}

		[Test]
		public async Task GetSubcategories_ReturnsTheCorrectSubcategories()
		{
			var categories = await mTestedService.GetSubcategories(4);

			var returnedSubCategoryNames = categories.Select(x => x.Name);
			var expectedSubCategoryNames = new string[]
			{
				"Hard drives", "Graphics cards", "Motherboards", "Processors", "RAM memory", "Power supplies", "Cases"
			};

			Assert.That(returnedSubCategoryNames, Is.EquivalentTo(expectedSubCategoryNames));
		}

		[Test]
		public async Task GetSubcategories_ReturnsAlsoCategoriesWithOnlyInactiveAuctions()
		{
			var categories = await mTestedService.GetSubcategories(1);

			var returnedSubCategoryNames = categories.Select(x => x.Name);
			var expectedSubCategoryNames = new string[] { "Desktop computers", "Mobile computers", "Components" };

			Assert.That(returnedSubCategoryNames, Is.EquivalentTo(expectedSubCategoryNames));
		}

		[Test]
		public async Task GetSubcategories_ReturnsAlsoCorrectInfoAboutAuctionCount()
		{
			var categories = await mTestedService.GetSubcategories(12);

			var returnedAuctionCount = categories.Select(x => x.AuctionCount);
			var expectedAuctionCount = new int[] { 0, 2, 0, 10 };

			Assert.That(returnedAuctionCount, Is.EqualTo(expectedAuctionCount));
		}
	}
}
