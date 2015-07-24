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
		private HttpRequestBase    mRequestMock;
		private HttpResponseBase   mResponseMock;

		[SetUp]
		public void SetUp()
		{
			mCategoryServiceMock = A.Fake<ICategoryService>();
			mAuctionServiceMock  = A.Fake<IAuctionService>();
			mRequestMock         = A.Fake<HttpRequestBase>();
			mResponseMock        = A.Fake<HttpResponseBase>();

			mTestedController = new CategoryController(mCategoryServiceMock, mAuctionServiceMock, mRequestMock, mResponseMock);

			var fakePagedList = A.Fake<IPagedList<Auction>>();
			A.CallTo(() => fakePagedList.PageNumber).Returns(1);
			A.CallTo(() => fakePagedList.PageSize).Returns(1);
			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(0, 0, 0)).WithAnyArguments().Returns(fakePagedList);
		}

		[Test]
		public async Task PageSizeIsReadFromACookieIfItsThere()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "30") });

			await mTestedController.Index(id: 2, page: 2, pageSize: null);

			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>.Ignored, A<int>.Ignored, 30))
			 .MustHaveHappened();
		}

		[Test]
		public async Task ExplicitlySettingPageSizeOverwritesTheValueFromACookie()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "30") });

			await mTestedController.Index(id: 2, page: 2, pageSize: 40);

			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>.Ignored, A<int>.Ignored, 40))
			 .MustHaveHappened();
			A.CallTo(() => mResponseMock.SetCookie(A<HttpCookie>.That.Matches(x => x.Value == "40")));
		}

		[Test]
		public async Task DoNotAllowMoreThan50ResultsPerPage()
		{
			var maxAllowedResultsPerPage = 50;

			await mTestedController.Index(id: 2, page: 2, pageSize: 1000);

			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>.Ignored, A<int>.Ignored, maxAllowedResultsPerPage))
			 .MustHaveHappened();
		}

		[Test]
		public async Task DoNotAllowMoreThan50ResultsPerPage_EvenIfTheValuesIsGottenFromACookie()
		{
			A.CallTo(() => mRequestMock.Cookies).Returns(new HttpCookieCollection { new HttpCookie("pageSize", "1000") });

			var maxAllowedResultsPerPage = 50;

			await mTestedController.Index(id: 2, page: 2, pageSize: null);

			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>.Ignored, A<int>.Ignored, maxAllowedResultsPerPage))
			 .MustHaveHappened();
		}
	}
}
