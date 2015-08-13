using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lang = Auctioneer.Resources.Admin.Users.Ban;

namespace Auctioneer.Presentation.Areas.Admin.Models
{
	public class UsersUnbanViewModel
	{
		[HiddenInput]
		public string Id { get; set; }

		[HiddenInput]
		public string ListQueryString { get; set; }

		[HiddenInput]
		public string UserName { get; set; }

		public string RealName { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan TimeTillBanEnd { get; set; }

		[DataType(DataType.MultilineText)]
		public string Reason { get; set; }
	}
}