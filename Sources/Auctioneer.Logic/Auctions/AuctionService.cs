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

using Ganss.XSS;

using PagedList;

using Auctioneer.Logic.Utils;

namespace Auctioneer.Logic.Auctions
{
	public class AuctionService : IAuctionService
	{
		private static readonly Size THUMBNAIL_SIZE = new Size(100, 100);

		private readonly AuctioneerDbContext mContext;
		private readonly string              mAuctionPhotoDirectoryPath;
		private readonly string              mAuctionThumbnailDirectoryPath;

		public AuctionService(AuctioneerDbContext context,
		                      string photoDirectoryPath,
		                      string thumbnailDirectoryPath)
		{
			Contract.Requires(context != null);
			Contract.Requires(!String.IsNullOrWhiteSpace(photoDirectoryPath));
			Contract.Requires(!String.IsNullOrWhiteSpace(thumbnailDirectoryPath));

			mContext                       = context;
			mAuctionPhotoDirectoryPath     = photoDirectoryPath;
			mAuctionThumbnailDirectoryPath = thumbnailDirectoryPath;
		}

		public Task<IPagedList<Auction>> GetActiveAuctionsInCategory(int categoryId,
		                                                             AuctionSortOrder sortBy,
		                                                             int pageIndex,
		                                                             int auctionsPerPage)
		{
			var auctions = from auction      in mContext.Auctions
			               from rootCategory in mContext.Categories
			               where rootCategory.Id == categoryId

			               join category in mContext.Categories
			               on auction.CategoryId equals category.Id
			               where auction.EndDate > DateTime.Now

			               where category.Left >= rootCategory.Left && category.Right <= rootCategory.Right
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
			                                .Where(x => x.CreationDate >= createdAfter);

			if(statusFilter.HasFlag(AuctionStatusFilter.Active) == false)
			{
				auctions = auctions.Where(x => x.EndDate <= DateTime.Now);
			}

			if(statusFilter.HasFlag(AuctionStatusFilter.Expired) == false)
			{
				auctions = auctions.Where(x => x.EndDate > DateTime.Now);
			}

			if(!String.IsNullOrWhiteSpace(titleFilter))
			{
				auctions = auctions.Where(x => x.Title.Contains(titleFilter));
			}

			auctions = auctions.OrderBy(x => x.Title);

			return Task.FromResult(auctions.ToPagedList(pageIndex, auctionsPerPage));
		}

		public async Task<IEnumerable<Auction>> GetRecentAuctions(int maxResults)
		{
			return await mContext.Auctions.OrderByDescending(x => x.CreationDate)
			                              .Take(maxResults)
			                              .ToListAsync();
		}

		public async Task<Auction> GetById(int id)
		{
			var auction = await mContext.Auctions.Include(x => x.Seller)
			                                     .SingleOrDefaultAsync(x => x.Id == id)
			                                     .ConfigureAwait(false);
			if(auction == null)
				throw new ObjectNotFoundException("Auction with id = " + id + " does not exist.");

			return auction;
		}

		public async Task AddAuction(Auction newAuction)
		{
			var sanitizer = new HtmlSanitizer();
			
			newAuction.Description = sanitizer.Sanitize(newAuction.Description);

			mContext.Auctions.Add(newAuction);
			await mContext.SaveChangesAsync();
		}

		public async Task StoreAuctionPhotos(int auctionId, IEnumerable<Stream> dataStreams)
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

		public async Task RemoveAuctions(string removingUserId, params int[] ids)
		{
			var auctions = await mContext.Auctions.Where(x => ids.Contains(x.Id)).ToListAsync();
			if(auctions.Any(x => x.SellerId != removingUserId))
				throw new LogicException("Cannot remove auctions of another user.");

			if(auctions.Any(x => x.EndDate <= DateTime.Now))
				throw new LogicException("Inactive auctions cannot be removed.");

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
	}
}
