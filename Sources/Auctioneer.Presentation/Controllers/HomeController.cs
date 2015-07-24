using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Mappers.Category;

namespace Auctioneer.Presentation.Controllers
{
	public class HomeController : Controller
	{
		private readonly IAuctionService mAuctionService;

		public HomeController(IAuctionService auctionService)
		{
			mAuctionService = auctionService;
		}

		public async Task<ActionResult> Index()
		{
			var auctions   = await mAuctionService.GetRecentAuctions(maxResults: 20);
			var viewModels = auctions.Select(AuctionViewModelMapper.FromAuction);

			return View(viewModels);
		}
	}
}