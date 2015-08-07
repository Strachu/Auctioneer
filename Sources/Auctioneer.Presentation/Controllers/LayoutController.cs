using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Controllers
{
	public class LayoutController : Controller
	{
		[ChildActionOnly]
		public PartialViewResult LoginLinks()
		{
			return (User.Identity.IsAuthenticated) ? PartialView("_LoginLinks.Authenticated")
			                                       : PartialView("_LoginLinks.NotAuthenticated");
		}
	}
}