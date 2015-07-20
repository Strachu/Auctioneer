﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
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
		private IAuctionService    mAuctionServiceMock;

		[SetUp]
		public void SetUp()
		{
			mCategoryServiceMock = A.Fake<ICategoryService>();
			mAuctionServiceMock  = A.Fake<IAuctionService>();

			mTestedController = new CategoryController(mCategoryServiceMock, mAuctionServiceMock);
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

			var returnedViewModel  = await mTestedController.Index(2).GetModel<CategoryIndexViewModel>();
			var returnedCategories = returnedViewModel.Category.Categories;

			Assert.That(returnedCategories, Is.Ordered.By(PropertyName.Of(() => returnedCategories.First().Name)));
		}

		[Test]
		public async Task WhenIdHasBeenGiven_IndexAction_ReturnsActiveAuctions()
		{
			var auctions = new Auction[]
			{
				new Auction { Title = "Auction #1", Price = 100 },
				new Auction { Title = "Auction #2", Price = 0.1m },
				new Auction { Title = "Auction #3", Price = 10m }
			};

			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(2)).Returns(auctions);

			var returnedViewModel = await mTestedController.Index(2).GetModel<CategoryIndexViewModel>();
			var returnedAuctions  = returnedViewModel.Auctions;

			// TODO assert Price
			Assert.That(returnedAuctions.Select(x => x.Title), Is.EquivalentTo(auctions.Select(x => x.Title)));
		}

		[Test]
		public void TopCategoriesAreSortedAlphabetically()
		{
			var categories = new Category[]
			{
				new Category { Name = "2FirstCategory" },
				new Category { Name = "1SecondCategory" },
				new Category { Name = "3LastCategory" },
				new Category { Name = "ZZZZ" }
			};

			A.CallTo(() => mCategoryServiceMock.GetTopLevelCategories()).Returns(categories);

			var returnedCategories = mTestedController.TopCategories().GetModel<CategoryListViewModel>().Categories;

			Assert.That(returnedCategories, Is.Ordered.By(PropertyName.Of(() => returnedCategories.First().Name)));
		}
	}
}
