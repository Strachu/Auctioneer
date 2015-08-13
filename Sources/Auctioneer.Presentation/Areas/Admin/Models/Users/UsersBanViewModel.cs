using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lang = Auctioneer.Resources.Admin.Users.Ban;

namespace Auctioneer.Presentation.Areas.Admin.Models
{
	public class UsersBanViewModel
	{
		public UsersBanViewModel()
		{
			BanDuration = TimeSpan.FromDays(1);
		}

		[HiddenInput]
		public string Id { get; set; }

		[HiddenInput]
		public string ListQueryString { get; set; }

		[HiddenInput]
		public string UserName { get; set; }

		public string RealName { get; set; }

		[Required]
		[DataType(DataType.Duration)]
		[Display(Name = "BanDuration", ResourceType = typeof(Lang))]
		public TimeSpan BanDuration { get; set; }

		[DataType(DataType.MultilineText)]
		[Display(Name = "Reason", ResourceType = typeof(Lang))]
		public string Reason { get; set; }
	}
}