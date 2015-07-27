using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Mappers;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Controllers
{
	public class AuctionController : Controller
	{
		private readonly IAuctionService mAuctionService;

		public AuctionController(IAuctionService auctionService)
		{
			Contract.Requires(auctionService != null);

			mAuctionService = auctionService;
		}

		[Route("Auction/{id}/{slug?}")]
		public async Task<ActionResult> Show(int id)
		{
			// TODO what if there's no auction with that id?
			var auction   = await mAuctionService.GetById(id);
			var viewModel = AuctionShowViewModelMapper.FromAuction(auction);

			return View(viewModel);
		}

		public ActionResult Breadcrumb(int id)
		{
			// TODO
			return PartialView("_Breadcrumb", new BreadcrumbViewModel { Items = new BreadcrumbViewModel.Item[0] });
		}
	}
}