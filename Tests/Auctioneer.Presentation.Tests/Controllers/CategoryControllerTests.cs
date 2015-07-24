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
	}
}
