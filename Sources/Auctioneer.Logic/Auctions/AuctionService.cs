using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Auctioneer.Logic.Users;
using Auctioneer.Logic.Validation;

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

		public Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId,
		                                                             AuctionSortOrder sortBy,
		                                                             int pageIndex,
		                                                             int auctionsPerPage)
		{
			var auctions = from auction      in mContext.Auctions.Where(AuctionStatusFilter.Active)
			               from rootCategory in mContext.Categories.Where(x => x.Id == categoryId)

			               from subCategory  in mContext.Categories
			               where subCategory.Left  >= rootCategory.Left &&
			                     subCategory.Right <= rootCategory.Right

			               where auction.CategoryId == subCategory.Id
			               select auction;

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
					auctions = auctions.OrderBy(x => x.Price);
					break;
				case AuctionSortOrder.PriceDescending:
					auctions = auctions.OrderByDescending(x => x.Price);
					break;
			}

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

			var auctions = mContext.Auctions.Include(x => x.Category)
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
			return await mContext.Auctions.Where(AuctionStatusFilter.Active)
			                              .OrderByDescending(x => x.CreationDate)
			                              .Take(maxResults)
			                              .ToListAsync();
		}

		public async Task<Auction> GetById(int id)
		{
			var auction = await mContext.Auctions.Include(x => x.Seller)
			                                     .Include(x => x.Buyer)
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

			mContext.Auctions.Add(newAuction);
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

			if(auction.Status != AuctionStatus.Active)
			{
				errors.AddError(Lang.Delete.AuctionIsInactive);
				return false;
			}

			return true;
		}

		public async Task RemoveAuctions(IReadOnlyCollection<int> ids, string removingUserId, IValidationErrorNotifier errors)
		{
			var auctions = await mContext.Auctions.Where(x => ids.Contains(x.Id)).ToListAsync();
			if(!await auctions.All(async x => await CanBeRemoved(x, removingUserId, errors)))
				return;

			// Cannot use an array with ExecuteSqlCommandAsync(), concatenating int elements should be safe.
			var sql = String.Format("DELETE FROM Auctions WHERE id IN ({0})", String.Join(",", ids));
			await mContext.Database.ExecuteSqlCommandAsync(sql);

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

		public async Task Buy(int auctionId, string buyerId, IValidationErrorNotifier errors)
		{
			var auction = await GetById(auctionId);
			if(!CanBeBought(auction, buyerId, errors))
				return;

			auction.BuyerId = buyerId;

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
