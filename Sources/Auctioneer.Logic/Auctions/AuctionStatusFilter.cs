using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	[Flags]
	public enum AuctionStatusFilter
	{
		Active  = 1 << 0,
		Expired = 1 << 1,
		
		All = Active | Expired
	}
}
