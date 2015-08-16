using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Utils;
using Auctioneer.Presentation.Controllers;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Models;
using Auctioneer.Presentation.Tests.TestUtils;

using FakeItEasy;

using NUnit.Framework;

using PagedList;

namespace Auctioneer.Presentation.Tests.Controllers
{
	[TestFixture]
	internal class CategoryControllerTests
	{
		private CategoryController mTestedController;
		private ICategoryService   mCategoryServiceMock;
		private IAuctionService    mAuctionServiceMock;
		private IBreadcrumbBuilder mBreadcrumbBuilderMock;
		private HttpRequestBase    mRequestMock;
		private HttpResponseBase   mResponseMock;

		[SetUp]
		public void SetUp()
		{
			mCategoryServiceMock   = A.Fake<ICategoryService>();
			mAuctionServiceMock    = A.Fake<IAuctionService>();
			mBreadcrumbBuilderMock = A.Fake<IBreadcrumbBuilder>();
			mRequestMock           = A.Fake<HttpRequestBase>();
			mResponseMock          = A.Fake<HttpResponseBase>();

			mTestedController = new CategoryController(mCategoryServiceMock, mAuctionServiceMock, mBreadcrumbBuilderMock,
			                                           mRequestMock, mResponseMock);

			var fakePagedList = A.Fake<IPagedList<Auction>>();
			A.CallTo(() => fakePagedList.PageNumber).Returns(1);
			A.CallTo(() => fakePagedList.PageSize).Returns(1);
			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(0, null, 0, 0, 0)).WithAnyArguments().Returns(fakePagedList);
		}

		[Test]
		public async Task PageSizeIsReadFromACookieIfItsThere()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "30") });

			await mTestedController.Index(id: 2, page: 2);

			AssertThatUsedPageSizeIsEqualTo(30);
		}

		[Test]
		public async Task ExplicitlySettingPageSizeOverwritesTheValueFromACookie()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "30") });

			await mTestedController.Index(id: 2, page: 2, pageSize: 40);

			AssertThatUsedPageSizeIsEqualTo(40);
			A.CallTo(() => mResponseMock.SetCookie(A<HttpCookie>.That.Matches(x => x.Value == "40")));
		}

		[Test]
		public async Task DoNotAllowMoreThan50ResultsPerPage()
		{
			await mTestedController.Index(id: 2, page: 2, pageSize: 1000);

			AssertThatUsedPageSizeIsEqualTo(50);
		}

		[Test]
		public async Task DoNotAllowMoreThan50ResultsPerPage_EvenIfTheValuesIsGottenFromACookie()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "1000") });

			await mTestedController.Index(id: 2, page: 2);

			AssertThatUsedPageSizeIsEqualTo(50);
		}

		[Test]
		public async Task WhenNotSearching_EmptyCategoriesAreShown()
		{
			A.CallTo(() => mCategoryServiceMock.GetSubcategories(0, null)).WithAnyArguments().Returns(new Category[]
			{
				new Category { Id = 1, AuctionCount = 10 },
				new Category { Id = 2, AuctionCount =  0 },
				new Category { Id = 3, AuctionCount =  1 },
			});

			var viewResult  = await mTestedController.Index(id: 1);
			var viewModel   = viewResult.GetModel<CategoryIndexViewModel>();
			var categoryIds = viewModel.Category.Categories.Select(x => x.Id);

			Assert.That(categoryIds, Is.EquivalentTo(new int[] { 1, 2, 3 }));
		}

		[Test]
		public async Task WhenSearching_EmptyCategoriesAreNotShown()
		{
			A.CallTo(() => mCategoryServiceMock.GetSubcategories(0, null)).WithAnyArguments().Returns(new Category[]
			{
				new Category { Id = 1, AuctionCount = 10 },
				new Category { Id = 2, AuctionCount =  0 },
				new Category { Id = 3, AuctionCount =  1 },
			});

			var viewResult  = await mTestedController.Index(id: 1, searchString: "Test");
			var viewModel   = viewResult.GetModel<CategoryIndexViewModel>();
			var categoryIds = viewModel.Category.Categories.Select(x => x.Id);

			Assert.That(categoryIds, Is.EquivalentTo(new int[] { 1, 3 }));
		}

		private void AssertThatUsedPageSizeIsEqualTo(int expectedPageSize)
		{
			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>.Ignored,
			                                                               A<string>.Ignored,
			                                                               A<AuctionSortOrder>.Ignored,
			                                                               A<int>.Ignored,
			                                                               expectedPageSize))
			 .MustHaveHappened();
		}
	}
}
