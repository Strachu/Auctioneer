using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Utils;
using Auctioneer.Presentation.Controllers;
using Auctioneer.Presentation.Models;
using Auctioneer.Presentation.Tests.TestUtils;

using FakeItEasy;

using NUnit.Framework;

namespace Auctioneer.Presentation.Tests.Controllers
{
	[TestFixture]
	internal class CategoryControllerTests
	{
		private CategoryController mTestedController;
		private ICategoryService   mCategoryServiceMock;

		[SetUp]
		public void SetUp()
		{
			mCategoryServiceMock = A.Fake<ICategoryService>();

			mTestedController = new CategoryController(mCategoryServiceMock);
		}

		[Test]
		public async Task WhenIdHasBeenNotGiven_IndexAction_ReturnsTopLevelCategories()
		{
			var topLevelCategories = new Category[]
			{
				new Category { Name = "FirstCategory" },
				new Category { Name = "SecondCategory" }
			};

			A.CallTo(() => mCategoryServiceMock.GetTopLevelCategories()).Returns(topLevelCategories);

			var returnedCategories    = await mTestedController.Index(null).GetModel<IEnumerable<CategoryViewModel>>();
			var returnedCategoryNames = returnedCategories.Select(x => x.Name);

			Assert.That(returnedCategoryNames, Is.EquivalentTo(topLevelCategories.Select(x => x.Name)));
		}

		[Test]
		public async Task WhenIdHasBeenGiven_IndexAction_ReturnsSubCategoriesOfCategoryWithThisId()
		{
			var category = new Category
			{
				SubCategories = new Category[]
				{
					new Category { Name = "FirstSubCategory" },
					new Category { Name = "SecondSubCategory" }
				}
			};

			A.CallTo(() => mCategoryServiceMock.GetCategoryById(2)).Returns(category);

			var returnedCategories    = await mTestedController.Index(2).GetModel<IEnumerable<CategoryViewModel>>();
			var returnedCategoryNames = returnedCategories.Select(x => x.Name);

			Assert.That(returnedCategoryNames, Is.EquivalentTo(category.SubCategories.Select(x => x.Name)));
		}

		[Test]
		public async Task TheCategoriesReturnedByTheControllerAreSortedAlphabetically()
		{
			var category = new Category
			{
				SubCategories = new Category[]
				{
					new Category { Name = "2FirstCategory" },
					new Category { Name = "1SecondCategory" },
					new Category { Name = "3LastCategory" },
					new Category { Name = "ZZZZ" }
				}
			};

			A.CallTo(() => mCategoryServiceMock.GetCategoryById(A<int>.Ignored)).Returns(category);

			var returnedCategories = await mTestedController.Index(2).GetModel<IEnumerable<CategoryViewModel>>();

			Assert.That(returnedCategories, Is.Ordered.By(PropertyName.Of(() => returnedCategories.First().Name)));
		}
	}
}
