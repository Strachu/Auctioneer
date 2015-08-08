using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lang = Auctioneer.Resources.Auction.Other;

namespace Auctioneer.Logic.Auctions
{
	public enum AuctionStatus
	{
		[Display(Name = "Status_Active", ResourceType = typeof(Lang))]
		Active,

		[Display(Name = "Status_Expired", ResourceType = typeof(Lang))]
		Expired,

		[Display(Name = "Status_Sold", ResourceType = typeof(Lang))]
		Sold,
	}
}
