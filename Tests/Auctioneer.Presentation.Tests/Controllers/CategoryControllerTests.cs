using System;
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

using PagedList;

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
		public async Task DoNotAllowMoreThan50ResultsPerPage()
		{
			var fakePagedList = A.Fake<IPagedList<Auction>>();
			A.CallTo(() => fakePagedList.PageNumber).Returns(1);
			A.CallTo(() => fakePagedList.PageSize).Returns(1);
			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(0, 0, 0)).WithAnyArguments().Returns(fakePagedList);

			var maxAllowedResultsPerPage = 50;

			await mTestedController.Index(id: 2, page: 2, pageSize: 1000);

			A.CallTo(() => mAuctionServiceMock.GetActiveAuctionsInCategory(A<int>.Ignored, A<int>.Ignored, maxAllowedResultsPerPage))
			 .MustHaveHappened();
		}
	}
}
