using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Auctioneer.Logic.Users;

using PagedList;

using Lang = Auctioneer.Resources.Admin.Users.Index;

namespace Auctioneer.Presentation.Areas.Admin.Models
{
	public class UsersIndexViewModel
	{
		public IPagedList<Item> Users { get; set; }

		public UserSortOrder CurrentSortOrder { get; set; }

		public class Item
		{
			public string Id { get; set; }

			[Display(Name = "UserName", ResourceType = typeof(Lang))]
			public string UserName { get; set; }

			[Display(Name = "RealName", ResourceType = typeof(Lang))]
			public string RealName { get; set; }

			public bool IsBanned { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "BanExpiryTime", ResourceType = typeof(Lang))]
			public DateTime? BanExpiryTime { get; set; }
		}
	}
}