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
			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(0, 0, 0, 0)).WithAnyArguments().Returns(fakePagedList);
		}

		[Test]
		public async Task PageSizeIsReadFromACookieIfItsThere()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "30") });

			await mTestedController.Index(id: 2, page: 2, pageSize: null, sortOrder: null);

			AssertThatUsedPageSizeIsEqualTo(30);
		}

		[Test]
		public async Task ExplicitlySettingPageSizeOverwritesTheValueFromACookie()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "30") });

			await mTestedController.Index(id: 2, page: 2, pageSize: 40, sortOrder: null);

			AssertThatUsedPageSizeIsEqualTo(40);
			A.CallTo(() => mResponseMock.SetCookie(A<HttpCookie>.That.Matches(x => x.Value == "40")));
		}

		[Test]
		public async Task DoNotAllowMoreThan50ResultsPerPage()
		{
			await mTestedController.Index(id: 2, page: 2, pageSize: 1000, sortOrder: null);

			AssertThatUsedPageSizeIsEqualTo(50);
		}

		[Test]
		public async Task DoNotAllowMoreThan50ResultsPerPage_EvenIfTheValuesIsGottenFromACookie()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "1000") });

			await mTestedController.Index(id: 2, page: 2, pageSize: null, sortOrder: null);

			AssertThatUsedPageSizeIsEqualTo(50);
		}

		private void AssertThatUsedPageSizeIsEqualTo(int expectedPageSize)
		{
			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>._,
			                                                               A<AuctionSortOrder>._,
			                                                               A<int>._,
			                                                               expectedPageSize))
			 .MustHaveHappened();
		}
	}
}
