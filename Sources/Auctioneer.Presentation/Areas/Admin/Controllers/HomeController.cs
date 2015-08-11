using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Areas.Admin.Controllers
{
	[Authorize(Roles="Admin")]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction(controllerName: "Users", actionName: "Index");
		}
	}
}