using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.ValueTypes;

namespace Auctioneer.Logic.TestDbData
{
	internal static class Auctions
	{
		public static void Add(AuctioneerDbContext context)
		{
			var rndGenerator     = new Random(Seed: 746293114);
			var imageInitializer = new AuctionImageInitializer();
			var categoryCount    = context.Categories.Count();
			var userIds          = context.Users.Select(x => x.Id).ToList();
			var currencies       = context.Currencies.ToList();

			var auctions = new Auction[100000];
			for(int i = 0; i < auctions.Length; ++i)
			{
				var creationDate = new DateTime
				(
					day    : rndGenerator.Next(1, 29),
					month  : rndGenerator.Next(1, 13),
					year   : rndGenerator.Next(2010, 2016), 
					hour   : rndGenerator.Next(0, 24),
					minute : rndGenerator.Next(0, 60),
					second : rndGenerator.Next(0, 60)
				);

				if(creationDate > DateTime.Now)
					creationDate = DateTime.Now;

				auctions[i] = new Auction
				{
					CategoryId   = rndGenerator.Next(categoryCount) + 1,
					Title        = "The auction #" + (i + 1),
					Description  = "The description of auction number " + (i + 1),
					SellerId     = userIds[rndGenerator.Next(userIds.Count)],
					CreationDate = creationDate,
					EndDate      = creationDate.AddDays(rndGenerator.NextDouble() * 14),
					Price        = new Money(rndGenerator.Next(1, 1000), currencies[rndGenerator.Next(currencies.Count)]),
					PhotoCount   = 0
				};

				bool sold = (rndGenerator.Next(100) + 1) < 75;
				if(sold)
				{
					auctions[i].BuyerId = userIds[rndGenerator.Next(userIds.Count)];
				}

				if(auctions[i].Status == AuctionStatus.Active)
				{
					imageInitializer.CopyRandomThumbnailForAuction(i + 1);
					imageInitializer.CopyRandomPhotosForAuction(i + 1);

					auctions[i].PhotoCount = imageInitializer.GetAuctionPhotoCount(i + 1);
				}
			}

			InsertAuctions(context, auctions);
		}

		private static void InsertAuctions(AuctioneerDbContext context, IList<Auction> auctions)
		{
			// It's the fastest way to correctly insert a lot of auctions.
			// Plain SQL Insert took more than 4 minutes while this method takes only 16 seconds.
			// EntityFramework.AddRange() was way too slow and BulkInsert requires foreign key property on entity to
			// relate the entities properly (which I don't want to add because it is not needed).

			InsertMoneys(context, auctions.Select(x => x.Price));

			var table = new DataTable();

			table.Columns.Add("Id",           typeof(int));
			table.Columns.Add("Title",        typeof(string));
			table.Columns.Add("Description",  typeof(string));
			table.Columns.Add("CreationDate", typeof(DateTime));
			table.Columns.Add("EndDate",      typeof(DateTime));
			table.Columns.Add("PhotoCount",   typeof(int));
			table.Columns.Add("CategoryId",   typeof(int));
			table.Columns.Add("SellerId",     typeof(string));
			table.Columns.Add("BuyerId",      typeof(string));
			table.Columns.Add("Price_Id",     typeof(int));

			for(int i = 0; i < auctions.Count ;++i)
			{
				var auction = auctions[i];
				var row     = table.NewRow();

				row[1] = auction.Title;
				row[2] = auction.Description;
				row[3] = auction.CreationDate;
				row[4] = auction.EndDate;
				row[5] = auction.PhotoCount;
				row[6] = auction.CategoryId;
				row[7] = auction.SellerId;
				row[8] = auction.BuyerId;
				row[9] = i + 1;

				table.Rows.Add(row);
			}

			var bulkInsert = new SqlBulkCopy(context.Database.Connection.ConnectionString, SqlBulkCopyOptions.TableLock)
			{
				DestinationTableName = "Auctions"
			};

			bulkInsert.WriteToServer(table);
		}

		private static void InsertMoneys(AuctioneerDbContext context, IEnumerable<Money> moneys)
		{
			var table = new DataTable();

			table.Columns.Add("Id",              typeof(int));
			table.Columns.Add("Amount",          typeof(decimal));
			table.Columns.Add("Currency_Symbol", typeof(string));

			foreach(var money in moneys)
			{
				var row = table.NewRow();

				row[1] = money.Amount;
				row[2] = money.Currency.Symbol;

				table.Rows.Add(row);
			}

			var bulkInsert = new SqlBulkCopy(context.Database.Connection.ConnectionString, SqlBulkCopyOptions.TableLock)
			{
				DestinationTableName = "Moneys"
			};

			bulkInsert.WriteToServer(table);
		}

		private class AuctionImageInitializer
		{
			private readonly Random mRandomGenerator = new Random(Seed: 746293114);

			private readonly string mSampleThumbnailsPath;
			private readonly string mSamplePhotosPath;
			private readonly string mThumbnailsPath;
			private readonly string mPhotosPath;

			private readonly bool   mCopyThumbnails = false;
			private readonly bool   mCopyPhotos = false;

			private readonly int    mSampleThumbnailsCount;
			private readonly int    mSamplePhotosCount;

			public AuctionImageInitializer()
			{
				var aspNetAppPath     = AppDomain.CurrentDomain.BaseDirectory;
				mSampleThumbnailsPath = Path.Combine(aspNetAppPath, "Content/SampleThumbnails");
				mSamplePhotosPath     = Path.Combine(aspNetAppPath, "Content/SamplePhotos");
				mThumbnailsPath       = Path.Combine(aspNetAppPath, "Content/UserContent/Auctions/Thumbnails");
				mPhotosPath           = Path.Combine(aspNetAppPath, "Content/UserContent/Auctions/Photos");

				Directory.CreateDirectory(mThumbnailsPath);
				Directory.CreateDirectory(mPhotosPath);

				if(Directory.Exists(mSampleThumbnailsPath) && !Directory.EnumerateFiles(mThumbnailsPath, "*.jpg").Any())
				{
					mCopyThumbnails        = true;
					mSampleThumbnailsCount = Directory.EnumerateFiles(mSampleThumbnailsPath, "*.jpg").Count();
				}

				if(Directory.Exists(mSamplePhotosPath) &&
				  !Directory.EnumerateFiles(mPhotosPath, "*.jpg", SearchOption.AllDirectories).Any())
				{
					mCopyPhotos        = true;
					mSamplePhotosCount = Directory.EnumerateFiles(mSamplePhotosPath, "*.jpg").Count();
				}
			}

			public void CopyRandomThumbnailForAuction(int auctionId)
			{
				if(!mCopyThumbnails)
					return;

				var thumbnailIndex      = mRandomGenerator.Next(mSampleThumbnailsCount) + 1;
				var sampleThumbnailPath = Path.Combine(mSampleThumbnailsPath, thumbnailIndex + ".jpg");
				var destinationPath     = Path.Combine(mThumbnailsPath, auctionId + ".jpg");

				File.Copy(sampleThumbnailPath, destinationPath, overwrite: false);
			}

			public void CopyRandomPhotosForAuction(int auctionId)
			{
				if(!mCopyPhotos)
					return;

				var alreadyCopied = new HashSet<int>();

				var photosToCopy = mRandomGenerator.Next(4) + 1;
				for(int i = 0; i < photosToCopy; ++i)
				{
					int photoIndex;
					do
					{
						photoIndex = mRandomGenerator.Next(mSamplePhotosCount) + 1;
					}
					while(alreadyCopied.Contains(photoIndex));

					alreadyCopied.Add(photoIndex);

					var samplePhotoPath      = Path.Combine(mSamplePhotosPath, photoIndex + ".jpg");
					var destinationDirectory = Path.Combine(mPhotosPath, auctionId.ToString());
					var destinationPath      = Path.Combine(destinationDirectory, i + ".jpg");

					Directory.CreateDirectory(destinationDirectory);

					File.Copy(samplePhotoPath, destinationPath, overwrite: false);
				}
			}

			public int GetAuctionPhotoCount(int auctionId)
			{
				var photosDirectory = Path.Combine(mPhotosPath, auctionId.ToString());
				if(!Directory.Exists(photosDirectory))
					return 0;

				return Directory.EnumerateFiles(photosDirectory, "*.jpg").Count();
			}
		}
	}
}
