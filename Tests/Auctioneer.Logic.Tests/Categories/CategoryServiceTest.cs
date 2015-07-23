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
				Name          = "Computers",
				SubCategories = new TestCategory[]
				{
					new TestCategory { Name = "Desktop computers" },
					new TestCategory { Name = "Mobile computers" },
					new TestCategory
					{
						Name          = "Components",
						SubCategories = new TestCategory[]
						{
							new TestCategory { Name = "Hard drives" },
							new TestCategory { Name = "Graphics cards" },
							new TestCategory { Name = "Motherboards" },
							new TestCategory { Name = "Processors" },
							new TestCategory { Name = "RAM memory" },
							new TestCategory { Name = "Power supplies" },
							new TestCategory { Name = "Cases" },
						}
					}
				}
			});

			context.Categories.Add(new TestCategory
			{
				Name          = "Software",
				SubCategories = new TestCategory[]
				{
					new TestCategory { Name = "Operating systems" },
					new TestCategory { Name = "Office" },
					new TestCategory { Name = "Security" },
					new TestCategory { Name = "Games" },
				}
			});

			context.SaveChanges();
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
		public async Task GetCategoryById_ReturnsTheCorrectCategory()
		{
			var category = await mTestedService.GetCategoryById(7);

			Assert.That(category.Name, Is.EqualTo("Motherboards"));
		}

		[Test]
		public async Task GetCategoryById_PopulatesTheSubcategoryList()
		{
			var category = await mTestedService.GetCategoryById(4);

			var returnedSubCategoryNames = category.SubCategories.Select(x => x.Name);
			var expectedSubCategoryNames = new string[]
			{
				"Hard drives", "Graphics cards", "Motherboards", "Processors", "RAM memory", "Power supplies", "Cases"
			};

			Assert.That(returnedSubCategoryNames, Is.EquivalentTo(expectedSubCategoryNames));
		}
	}
}
