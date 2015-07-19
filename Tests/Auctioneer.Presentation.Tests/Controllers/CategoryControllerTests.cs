using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Categories;
using Auctioneer.Presentation.Controllers;
using Auctioneer.Presentation.Models;

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

			var viewResult            = await mTestedController.Index(null) as ViewResult;
			var returnedCategories    = viewResult.Model as IEnumerable<CategoryViewModel>;
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

			var viewResult            = await mTestedController.Index(2) as ViewResult;
			var returnedCategories    = viewResult.Model as IEnumerable<CategoryViewModel>;
			var returnedCategoryNames = returnedCategories.Select(x => x.Name);

			Assert.That(returnedCategoryNames, Is.EquivalentTo(category.SubCategories.Select(x => x.Name)));
		}
	}
}
