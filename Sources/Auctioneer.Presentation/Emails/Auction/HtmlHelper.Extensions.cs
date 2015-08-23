using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Html;

namespace Auctioneer.Presentation.Emails
{
	public static class HtmlHelperExtensions
	{
		public static IHtmlString AuctionLink(this HtmlHelper html,
		                                      int auctionId,
		                                      string auctionTitle)
		{
			var currentUrl = html.ViewContext.HttpContext.Request.Url;

			return html.ActionLinkWithSlug(auctionTitle, controllerName: "Auction", actionName: "Show",
			                               protocol: currentUrl.Scheme,
			                               routeValues: new
			                               {
				                               id = auctionId
			                               });
		}
	}
}