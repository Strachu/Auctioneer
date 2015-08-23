using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Users;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	public class EmailUserNotifier : IUserNotifier
	{
		private readonly IEmailService mMailService;

		public EmailUserNotifier(IEmailService mailService)
		{
			Contract.Requires(mailService != null);

			mMailService = mailService;
		}

		public async Task SendActivationToken(User user, string token)
		{
			await mMailService.SendAsync(new ConfirmationMail
			{
				UserMail      = user.Email,
				UserFirstName = user.FirstName,
				UserId        = user.Id,
				ConfirmToken  = token
			});
		}

		public async Task SendPasswordResetToken(User user, string token)
		{
			await mMailService.SendAsync(new ForgotPasswordMail
			{
				UserMail           = user.Email,
				UserFirstName      = user.FirstName,
				PasswordResetToken = token
			});
		}
		
		public async Task NotifyAuctionExpired(User user, Auction auction)
		{
			await mMailService.SendAsync(new AuctionExpiredMail
			{
				UserMail             = user.Email,
				UserFirstName        = user.FirstName,

				AuctionId            = auction.Id,
				AuctionTitle         = auction.Title
			});
		}

		public async Task NotifyAuctionSold(User user, Auction auction)
		{
			await mMailService.SendAsync(new AuctionSoldMail
			{
				UserMail             = user.Email,
				UserFirstName        = user.FirstName,

				AuctionId            = auction.Id,
				AuctionTitle         = auction.Title,
				AuctionPrice         = auction.BestOffer.Money,

				BuyerEmail           = auction.Buyer.Email,
				BuyerUserName        = auction.Buyer.UserName,
				BuyerFullName        = auction.Buyer.FirstName + " " + auction.Buyer.LastName,
				BuyerShippingAddress = auction.Buyer.Address,
			});
		}

		public async Task NotifyAuctionWon(User user, Auction auction)
		{
			await mMailService.SendAsync(new AuctionWonMail
			{
				UserMail       = user.Email,
				UserFirstName  = user.FirstName,

				AuctionId      = auction.Id,
				AuctionTitle   = auction.Title,
				AuctionPrice   = auction.BestOffer.Money,

				SellerEmail    = auction.Seller.Email,
				SellerUserName = auction.Seller.UserName,
				SellerFullName = auction.Seller.FirstName + " " + auction.Seller.LastName,
			});
		}

		public async Task NotifyOfferAdded(User user, BuyOffer offer, Auction auction)
		{
			await mMailService.SendAsync(new OfferAddedMail
			{
				UserMail       = user.Email,
				UserFirstName  = user.FirstName,

				AuctionId      = auction.Id,
				AuctionTitle   = auction.Title,

				OfferMoney     = offer.Money
			});
		}

		public async Task NotifyOutbid(User user, Auction auction)
		{
			await mMailService.SendAsync(new OutbidMail
			{
				UserMail       = user.Email,
				UserFirstName  = user.FirstName,

				AuctionId      = auction.Id,
				AuctionTitle   = auction.Title,
			});
		}
	}
}