using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Helpers
{
	public static class UrlHelperExtensions
	{
		public static string AuctionThumbnail(this UrlHelper url, int auctionId)
		{
			var fileName = auctionId + ".jpg";

			return url.Content("~/Content/Auctions/Thumbnails/" + fileName);
		}
	}
}