using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;

namespace Auctioneer.Logic.Users
{
	[ContractClass(typeof(IUserNotifierContractClass))]
	public interface IUserNotifier
	{
		Task SendActivationToken(User user, string token);
		Task SendPasswordResetToken(User user, string token);

		Task NotifyAuctionExpired(User user, Auction auction);
		Task NotifyAuctionSold(User user, Auction auction);
		Task NotifyAuctionWon(User user, Auction auction);

		Task NotifyOfferAdded(User user, BuyOffer offer, Auction auction);
		Task NotifyOutbid(User user, Auction auction);
	}
}
