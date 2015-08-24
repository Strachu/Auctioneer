using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;

namespace Auctioneer.Logic.Users
{
	[ContractClassFor(typeof(IUserNotifier))]
	internal abstract class IUserNotifierContractClass : IUserNotifier
	{
		Task IUserNotifier.SendActivationToken(User user, string token)
		{
			Contract.Requires(user != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(token));

			throw new NotImplementedException();
		}

		Task IUserNotifier.SendPasswordResetToken(User user, string token)
		{
			Contract.Requires(user != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(token));

			throw new NotImplementedException();
		}

		Task IUserNotifier.NotifyAuctionExpired(User user, Auctions.Auction auction)
		{
			Contract.Requires(user != null);
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		Task IUserNotifier.NotifyAuctionSold(User user, Auctions.Auction auction)
		{
			Contract.Requires(user != null);
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		Task IUserNotifier.NotifyAuctionWon(User user, Auctions.Auction auction)
		{
			Contract.Requires(user != null);
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		Task IUserNotifier.NotifyOfferAdded(User user, BuyOffer offer, Auctions.Auction auction)
		{
			Contract.Requires(user != null);
			Contract.Requires(offer != null);
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		Task IUserNotifier.NotifyOutbid(User user, Auctions.Auction auction)
		{
			Contract.Requires(user != null);
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}
	}
}
