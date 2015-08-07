using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lang = Auctioneer.Resources.Auction.Other;

namespace Auctioneer.Logic.Auctions
{
	[Flags]
	public enum AuctionStatusFilter
	{
		[Display(Name = "Status_Active", ResourceType = typeof(Lang))]
		Active  = 1 << 0,

		[Display(Name = "Status_Expired", ResourceType = typeof(Lang))]
		Expired = 1 << 1,
		
		All = Active | Expired
	}
}
