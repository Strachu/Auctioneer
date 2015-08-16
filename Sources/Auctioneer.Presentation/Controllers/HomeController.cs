using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Helpers;
using Auctioneer.Presentation.Mappers.Category;

using DevTrends.MvcDonutCaching;

namespace Auctioneer.Presentation.Controllers
{
	public class HomeController : Controller
	{
		private readonly IAuctionService    mAuctionService;
		private readonly IBreadcrumbBuilder mBreadcrumbBuilder;

		public HomeController(IAuctionService auctionService, IBreadcrumbBuilder breadcrumbBuilder)
		{
			mAuctionService    = auctionService;
			mBreadcrumbBuilder = breadcrumbBuilder;
		}

		[DonutOutputCache(Duration = 10)]
		public async Task<ActionResult> Index()
		{
			var auctions   = await mAuctionService.GetRecentAuctions(maxResults: 20);
			var viewModels = auctions.Select(AuctionViewModelMapper.FromAuction);

			return View(viewModels);
		}

		public ActionResult Search(string searchString)
		{
			return RedirectToAction(controllerName: "Category", actionName: "Index", routeValues: new { searchString });
		}

		[ChildActionOnly]
		public PartialViewResult Breadcrumb()
		{
			var breadcrumb = mBreadcrumbBuilder.WithHomepageLink().Build();

			return PartialView("_Breadcrumb", breadcrumb);
		}
	}
}