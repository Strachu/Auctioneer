using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Categories;

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
			var context = new AuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(context);

			mTestedService = new CategoryService(context);
		}

		private void AddTestData(AuctioneerDbContext context)
		{
			context.Categories.Add(new Category
			{
				Name          = "Computers",
				SubCategories = new Category[]
				{
					new Category { Name = "Desktop computers" },
					new Category { Name = "Mobile computers" },
					new Category
					{
						Name          = "Components",
						SubCategories = new Category[]
						{
							new Category { Name = "Hard drives" },
							new Category { Name = "Graphics cards" },
							new Category { Name = "Motherboards" },
							new Category { Name = "Processors" },
							new Category { Name = "RAM memory" },
							new Category { Name = "Power supplies" },
							new Category { Name = "Cases" },
						}
					}
				}
			});

			context.Categories.Add(new Category
			{
				Name          = "Software",
				SubCategories = new Category[]
				{
					new Category { Name = "Operating systems" },
					new Category { Name = "Office" },
					new Category { Name = "Security" },
					new Category { Name = "Games" },
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
