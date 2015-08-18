using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Auctions
{
	internal static class IQueryableAuctionExtensions
	{
		public static IQueryable<Auction> Where(this IQueryable<Auction> auctionQuery, AuctionStatusFilter allowedStatuses)
		{
			Contract.Requires(auctionQuery != null);

			if(allowedStatuses.HasFlag(AuctionStatusFilter.Active) == false)
			{
				auctionQuery = auctionQuery.Where(x => !
				(
					 x.EndDate > DateTime.Now && 
					(x.BuyoutPrice == null || x.Offers.All(offer => offer.Amount < x.BuyoutPrice.Amount)))
				);
			}

			if(allowedStatuses.HasFlag(AuctionStatusFilter.Expired) == false)
			{
				auctionQuery = auctionQuery.Where(x => !(x.EndDate <= DateTime.Now && !x.Offers.Any()));
			}

			if(allowedStatuses.HasFlag(AuctionStatusFilter.Sold) == false)
			{
				auctionQuery = auctionQuery.Where(x => !
				(
					(x.EndDate <= DateTime.Now && x.Offers.Any()) ||
				    x.Offers.Any(offer => offer.Amount >= x.BuyoutPrice.Amount))
				);
			}

			return auctionQuery;
		}

		public static IOrderedQueryable<Auction> OrderByMinimumPrice(this IQueryable<Auction> auctionQuery)
		{
			Contract.Requires(auctionQuery != null);

			return auctionQuery.OrderBy(MinimumPrice).ThenBy(x => x.BuyoutPrice.Amount);
		}

		public static IOrderedQueryable<Auction> OrderByMinimumPriceDescending(this IQueryable<Auction> auctionQuery)
		{
			Contract.Requires(auctionQuery != null);

			return auctionQuery.OrderByDescending(MinimumPrice).ThenByDescending(x => x.BuyoutPrice.Amount);;
		}

		private static Expression<Func<Auction, decimal>> MinimumPrice
		{
			get
			{
				return x => (x.MinBid == null) ? x.BuyoutPrice.Amount
				                               : (x.Offers.Any() ? x.Offers.Max(y => y.Amount) : x.MinBid.Amount);
			}
		}
	}
}
