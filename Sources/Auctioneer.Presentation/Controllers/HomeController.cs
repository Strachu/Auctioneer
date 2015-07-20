using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

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
			var auctions = await mAuctionService.GetRecentAuctions(20);

			var viewModels = new CategoryListViewModel
			{
				Auctions = auctions.Select(x => new AuctionViewModel
				{
					Id          = x.Id,
					Title       = x.Title,
					Price       = x.Price,
					TimeTillEnd = x.EndDate - DateTime.Now
				})
			};

			return View(viewModels);
		}
	}
}