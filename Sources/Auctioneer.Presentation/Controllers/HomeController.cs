using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var auctions   = new Auction[0]; // TODO

			var viewModels = new CategoryListViewModel
			{
				Auctions = auctions.Select(x => new AuctionViewModel
				{
					Id = x.Id,
					Title = x.Title,
					Price = x.Price,
					TimeTillEnd = DateTime.Now - x.EndDate
				})
			};

			return View(viewModels);
		}
	}
}