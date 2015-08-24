using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using EntityFramework.Extensions;

using Auctioneer.Logic.Categories;
using Auctioneer.Logic.Users;
using Auctioneer.Logic.Validation;
using Auctioneer.Logic.ValueTypes;

using Ganss.XSS;

using PagedList;

using Auctioneer.Logic.Utils;

using Lang = Auctioneer.Resources.Auction;

namespace Auctioneer.Logic.Auctions
{
	public class AuctionService : IAuctionService
	{
		private static readonly Size THUMBNAIL_SIZE = new Size(100, 100);

		private readonly AuctioneerDbContext mContext;
		private readonly IUserNotifier       mUserNotifier;
		private readonly IUserService        mUserService;
		private readonly string              mAuctionPhotoDirectoryPath;
		private readonly string              mAuctionThumbnailDirectoryPath;

		public AuctionService(AuctioneerDbContext context,
		                      IUserNotifier userNotifier,
		                      IUserService userService,
		                      string photoDirectoryPath,
		                      string thumbnailDirectoryPath)
		{
			Contract.Requires(context != null);
			Contract.Requires(userNotifier != null);
			Contract.Requires(userService != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(photoDirectoryPath));
			Contract.Requires(!String.IsNullOrWhiteSpace(thumbnailDirectoryPath));

			mContext                       = context;
			mUserNotifier                  = userNotifier;
			mUserService                   = userService;
			mAuctionPhotoDirectoryPath     = photoDirectoryPath;
			mAuctionThumbnailDirectoryPath = thumbnailDirectoryPath;
		}

		public Task<IPagedList<Auction>> GetAllActiveAuctions(string titleFilter,
		                                                      AuctionSortOrder sortBy,
		                                                      int pageIndex,
		                                                      int auctionsPerPage)
		{
			return GetAuctions(cat => cat.ParentId == null, titleFilter, sortBy, pageIndex, auctionsPerPage);
		}

		public Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId,
		                                                             string titleFilter,
		                                                             AuctionSortOrder sortBy,
		                                                             int pageIndex,
		                                                             int auctionsPerPage)
		{
			return GetAuctions(cat => cat.Id == categoryId, titleFilter, sortBy, pageIndex, auctionsPerPage);
		}

		public Task<IPagedList<Auction>> GetAuctions(Expression<Func<Category, bool>> rootCategoryFilter,
		                                             string titleFilter,
		                                             AuctionSortOrder sortBy,
		                                             int pageIndex,
		                                             int auctionsPerPage)
		{
			var auctions = from auction      in mContext.Auctions.Where(AuctionStatusFilter.Active)
			               from rootCategory in mContext.Categories.Where(rootCategoryFilter)

			               from subCategory  in mContext.Categories
			               where subCategory.Left  >= rootCategory.Left &&
			                     subCategory.Right <= rootCategory.Right

			               where auction.CategoryId == subCategory.Id
			               select auction;

			if(!String.IsNullOrWhiteSpace(titleFilter))
			{
				auctions = auctions.Where(x => x.Title.Contains(titleFilter));
			}

			switch(sortBy)
			{
				case AuctionSortOrder.TitleAscending:
					auctions = auctions.OrderBy(x => x.Title);
					break;
				case AuctionSortOrder.TitleDescending:
					auctions = auctions.OrderByDescending(x => x.Title);
					break;

				default: // AuctionSortOrder.EndDateAscending
					auctions = auctions.OrderBy(x => x.EndDate);
					break;
				case AuctionSortOrder.EndDateDescending:
					auctions = auctions.OrderByDescending(x => x.EndDate);
					break;

				case AuctionSortOrder.PriceAscending:
					// TODO sorting should take into account relative value of currencies
					auctions = auctions.OrderByMinimumPrice();
					break;
				case AuctionSortOrder.PriceDescending:
					auctions = auctions.OrderByMinimumPriceDescending();
					break;
			}

			auctions = auctions.Include(x => x.MinimumPrice.Currency)
			                   .Include(x => x.BuyoutPrice.Currency)
			                   .Include(x => x.Offers);

			return Task.FromResult(auctions.ToPagedList(pageIndex, auctionsPerPage));
		}

		public Task<IPagedList<Auction>> GetAuctionsByUser(string userId,
		                                                   TimeSpan createdIn,
		                                                   string titleFilter,
		                                                   AuctionStatusFilter statusFilter,
		                                                   int pageIndex,
		                                                   int auctionsPerPage)
		{
			var createdAfter = DateTime.Now.Subtract(createdIn);

			var auctions = mContext.Auctions.Include(x => x.MinimumPrice.Currency)
			                                .Include(x => x.BuyoutPrice.Currency)
			                                .Include(x => x.Offers)
			                                .Include(x => x.Category)
			                                .Where(x => x.SellerId == userId)
			                                .Where(x => x.CreationDate >= createdAfter)
			                                .Where(allowedStatuses: statusFilter);

			if(!String.IsNullOrWhiteSpace(titleFilter))
			{
				auctions = auctions.Where(x => x.Title.Contains(titleFilter));
			}

			auctions = auctions.OrderBy(x => x.Title);

			return Task.FromResult(auctions.ToPagedList(pageIndex, auctionsPerPage));
		}

		public async Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults)
		{
			return await mContext.Auctions.Include(x => x.MinimumPrice.Currency)
			                              .Include(x => x.BuyoutPrice.Currency)
			                              .Include(x => x.Offers)
			                              .Where(AuctionStatusFilter.Active)
			                              .OrderByDescending(x => x.CreationDate)
			                              .Take(maxResults)
			                              .ToListAsync();
		}

		public async Task<Auction> GetById(int id)
		{
			var auction = await mContext.Auctions.Include(x => x.MinimumPrice.Currency)
			                                     .Include(x => x.BuyoutPrice.Currency)
			                                     .Include(x => x.Offers)
			                                     .Include(x => x.Seller)
			                                     .SingleOrDefaultAsync(x => x.Id == id)
			                                     .ConfigureAwait(false);
			if(auction == null)
				throw new ObjectNotFoundException("Auction with id = " + id + " does not exist.");

			return auction;
		}

		public async Task AddAuction(Auction newAuction, IEnumerable<Stream> photosData)
		{
			using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				await AddAuctionToDatabase(newAuction);
				await SaveAuctionPhotosToFileSystem(newAuction.Id, photosData);

				transaction.Complete();
			}
		}

		private async Task AddAuctionToDatabase(Auction newAuction)
		{
			var sanitizer = new HtmlSanitizer();
			
			newAuction.Description = sanitizer.Sanitize(newAuction.Description);

			await InsertAuction(newAuction);
		}

		private async Task InsertAuction(Auction auction)
		{
			// Do not insert new currencies
			if(auction.MinimumPrice != null)
			{
				mContext.Entry(auction.MinimumPrice.Currency).State = EntityState.Unchanged; 
			}

			if(auction.BuyoutPrice != null)
			{
				mContext.Entry(auction.BuyoutPrice.Currency).State = EntityState.Unchanged;
			}

			mContext.Auctions.Add(auction);
			await mContext.SaveChangesAsync();
		}

		private async Task SaveAuctionPhotosToFileSystem(int auctionId, IEnumerable<Stream> dataStreams)
		{
			var currentAuctionPhotosDirectory = Path.Combine(mAuctionPhotoDirectoryPath, auctionId.ToString());
			
			Directory.CreateDirectory(currentAuctionPhotosDirectory);

			await SavePhotos(currentAuctionPhotosDirectory, dataStreams);

			SaveThumbnailForFirstPhoto(currentAuctionPhotosDirectory, auctionId);
		}

		private async Task SavePhotos(string photosDirectory, IEnumerable<Stream> dataStreams)
		{
			int photoIndex = 0;
			foreach(var photoStream in dataStreams)
			{
				var destinationPath = Path.Combine(photosDirectory, photoIndex + ".jpg");

				using(var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					await photoStream.CopyToAsync(destinationStream).ConfigureAwait(false);
				}

				++photoIndex;
			}
		}

		private void SaveThumbnailForFirstPhoto(string photosDirectory, int auctionId)
		{
			var firstPhotoPath = Path.Combine(photosDirectory, "0.jpg");
			var firstPhotoData = new FileStream(firstPhotoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var firstPhoto     = new Bitmap(firstPhotoData);

			var thumbnail      = firstPhoto.ScaleToFitPreservingAspectRatio(THUMBNAIL_SIZE);
			var thumbnailPath  = Path.Combine(mAuctionThumbnailDirectoryPath, auctionId + ".jpg");

			thumbnail.Save(thumbnailPath, ImageFormat.Jpeg);			
		}

		public Task<bool> CanBeRemoved(Auction auction, string userId)
		{
			return CanBeRemoved(auction, userId, new ErrorCollection());
		}

		private async Task<bool> CanBeRemoved(Auction auction, string userId, IValidationErrorNotifier errors)
		{
			if(auction.SellerId != userId && !await mUserService.IsUserInRole(userId, "Admin"))
			{
				errors.AddError(Lang.Delete.WrongUser);
				return false;
			}

			if(auction.Offers.Any())
			{
				errors.AddError(Lang.Delete.BuyOfferHasBeenMade);
				return false;				
			}

			if(auction.Status != AuctionStatus.Active)
			{
				errors.AddError(Lang.Delete.AuctionIsInactive);
				return false;
			}

			return true;
		}

		public async Task RemoveAuctions(IReadOnlyCollection<int> ids, string removingUserId, IValidationErrorNotifier errors)
		{
			var auctions = await mContext.Auctions.Include(x => x.Offers).Where(x => ids.Contains(x.Id)).ToListAsync();
			if(!await auctions.All(async x => await CanBeRemoved(x, removingUserId, errors)))
				return;

			await mContext.Auctions.Where(x => ids.Contains(x.Id)).DeleteAsync();

			foreach(var auctionId in ids)
			{
				RemoveAuctionPhotos(auctionId);
			}
		}

		private void RemoveAuctionPhotos(int auctionId)
		{
			var currentAuctionPhotosDirectory = Path.Combine(mAuctionPhotoDirectoryPath,     auctionId.ToString());
			var thumbnailPath                 = Path.Combine(mAuctionThumbnailDirectoryPath, auctionId + ".jpg");
		
			File.Delete(thumbnailPath);
			Directory.Delete(currentAuctionPhotosDirectory, recursive: true);
		}

		public bool CanBeBought(Auction auction, string buyerId)
		{
			return CanBeBought(auction, buyerId, new ErrorCollection());
		}

		private bool CanBeBought(Auction auction, string buyerId, IValidationErrorNotifier errors)
		{
			if(auction.Status != AuctionStatus.Active)
			{
				errors.AddError(Lang.Buy.AuctionIsInactive);
				return false;
			}

			if(auction.SellerId == buyerId)
			{
				errors.AddError(Lang.Buy.CannotBuyOwnAuctions);
				return false;
			}

			return true;
		}

		public async Task Bid(int auctionId, string buyerId, decimal bidAmount, IValidationErrorNotifier errors)
		{
			var auction = await GetById(auctionId);
			if(!auction.IsBiddingEnabled)
			{
				errors.AddError(Lang.Buy.BiddingNotEnabled);
				return;
			}

			if(!CanBeBought(auction, buyerId, errors))
				return;

			if(bidAmount < auction.MinBidAllowed)
			{
				errors.AddError(String.Format(Lang.Buy.TooLowBid, new Money(auction.MinBidAllowed, auction.MinimumPrice.Currency)));
				return;
			}

			var previouslyBestOffer = auction.BestOffer;
			var userOffer           = new BuyOffer
			{
				UserId = buyerId,
				Date   = DateTime.Now,
				Amount = bidAmount,
			};

			auction.Offers.Add(userOffer);

			await mContext.SaveChangesAsync();

			await mUserNotifier.NotifyOfferAdded(userOffer.User, userOffer, auction);

			if(previouslyBestOffer != null)
			{
				await mUserNotifier.NotifyOutbid(previouslyBestOffer.User, auction);
			}
		}

		public async Task Buyout(int auctionId, string buyerId, IValidationErrorNotifier errors)
		{
			var auction = await GetById(auctionId);
			if(!auction.IsBuyoutEnabled)
			{
				errors.AddError(Lang.Buy.BuyoutNotEnabled);
				return;
			}

			if(!CanBeBought(auction, buyerId, errors))
				return;
			
			auction.Offers.Add(new BuyOffer
			{
				UserId = buyerId,
				Date   = DateTime.Now,
				Amount = auction.BuyoutPrice.Amount,
			});

			await mContext.SaveChangesAsync();

			await mUserNotifier.NotifyAuctionSold(auction.Seller, auction);
			await mUserNotifier.NotifyAuctionWon(auction.Buyer,   auction);
		}

		public Task<bool> CanBeMoved(Auction auction, string userId)
		{
			return mUserService.IsUserInRole(userId, "Admin");
		}

		public async Task MoveAuction(int auctionId, int newCategoryId, string movingUserId, IValidationErrorNotifier errors)
		{
			var auction = await this.GetById(auctionId);
			if(!await CanBeMoved(auction, movingUserId))
			{
				errors.AddError(Lang.Show.CannotMove);
				return;
			}

			auction.CategoryId = newCategoryId;

			await mContext.SaveChangesAsync();
		}
	}
}
