using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityFramework.BulkInsert.Extensions;

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
					PhotoCount   = 0
				};

				bool hasBuyoutPrice = (rndGenerator.Next(100) + 1) < 90;
				if(hasBuyoutPrice)
				{
					auctions[i].BuyoutPrice = new Money(rndGenerator.Next(1, 1000), currencies[rndGenerator.Next(currencies.Count)]);
				}

				bool isBiddingEnabled = !hasBuyoutPrice || ((rndGenerator.Next(100) + 1) < 30 && (auctions[i].BuyoutPrice.Amount >= 10.0m));
				if(isBiddingEnabled)
				{
					auctions[i].MinimumPrice = new Money(rndGenerator.Next(1, 1000), currencies[rndGenerator.Next(currencies.Count)]);
				}

				if(isBiddingEnabled && hasBuyoutPrice)
				{
					var fixedAmount = Math.Min(auctions[i].MinimumPrice.Amount, auctions[i].BuyoutPrice.Amount * 0.5m);

					auctions[i].MinimumPrice = new Money(fixedAmount, auctions[i].BuyoutPrice.Currency);
				}

				bool hasBidOffer = (rndGenerator.Next(100) + 1) < 90;
				if(isBiddingEnabled && hasBidOffer)
				{
					var offerCountProbability = new int[100];
					for(int x =  0; x <  30; ++x) offerCountProbability[x] = 1;
					for(int x = 30; x <  60; ++x) offerCountProbability[x] = 2;
					for(int x = 60; x <  80; ++x) offerCountProbability[x] = 3;
					for(int x = 80; x <  95; ++x) offerCountProbability[x] = 4;
					for(int x = 95; x < 100; ++x) offerCountProbability[x] = 5;

					var offerCount          = offerCountProbability[rndGenerator.Next(offerCountProbability.Length)];
					var previousOfferDate   = auctions[i].CreationDate.AddDays(1);
					var previousOfferAmount = 0.0m;
					for(int x = 0; x < offerCount; ++x)
					{
						var minimumPossibleOffer = Math.Max(Math.Ceiling(auctions[i].MinimumPrice.Amount), previousOfferAmount + 1);
						var maximumPossibleOffer = Math.Floor(hasBuyoutPrice ? auctions[i].BuyoutPrice.Amount * 0.9m : 1000.0m);
						if(minimumPossibleOffer >= maximumPossibleOffer)
							break;

						var offer = new BuyOffer
						{
							AuctionId = i + 1,
							UserId    = userIds[rndGenerator.Next(userIds.Count)],
							Date      = previousOfferDate.AddHours(rndGenerator.Next(1, 24)),
							Amount    = rndGenerator.Next((int)minimumPossibleOffer, (int)maximumPossibleOffer)
						};

						previousOfferDate   = offer.Date;
						previousOfferAmount = offer.Amount;

						auctions[i].Offers.Add(offer);
					}
				}

				bool hasBeenBoughtOut = (rndGenerator.Next(100) + 1) < 75;
				if(hasBeenBoughtOut && hasBuyoutPrice)
				{
					var lastOfferDate = (auctions[i].Offers.Any()) ? auctions[i].Offers.Last().Date : auctions[i].CreationDate;

					var buyoutOffer = new BuyOffer
					{
						AuctionId = i + 1,
						UserId    = userIds[rndGenerator.Next(userIds.Count)],
						Date      = lastOfferDate.AddHours(rndGenerator.Next(1, 24)),
						Amount    = auctions[i].BuyoutPrice.Amount
					};
					
					auctions[i].Offers.Add(buyoutOffer);
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

			var minPrices    = auctions.Select(x => x.MinimumPrice).Where(x => x != null).ToList();
			var buyoutPrices = auctions.Select(x => x.BuyoutPrice).Where(x => x != null).ToList();
			InsertMoneys(context, minPrices.Concat(buyoutPrices));

			var table = new DataTable("Auctions");

			table.Columns.Add("Id",              typeof(int));
			table.Columns.Add("Title",           typeof(string));
			table.Columns.Add("Description",     typeof(string));
			table.Columns.Add("CreationDate",    typeof(DateTime));
			table.Columns.Add("EndDate",         typeof(DateTime));
			table.Columns.Add("PhotoCount",      typeof(int));
			table.Columns.Add("CategoryId",      typeof(int));
			table.Columns.Add("SellerId",        typeof(string));
			table.Columns.Add("MinimumPrice_Id", typeof(int));
			table.Columns.Add("BuyoutPrice_Id",  typeof(int));

			int minPriceId    = 1;
			int buyoutPriceId = minPrices.Count + 1;
			foreach(var auction in auctions)
			{
				var row = table.NewRow();

				row[1] = auction.Title;
				row[2] = auction.Description;
				row[3] = auction.CreationDate;
				row[4] = auction.EndDate;
				row[5] = auction.PhotoCount;
				row[6] = auction.CategoryId;
				row[7] = auction.SellerId;

				if(auction.MinimumPrice != null)
				{
					row[8] = minPriceId++;
				}

				if(auction.BuyoutPrice != null)
				{
					row[9] = buyoutPriceId++;
				}

				table.Rows.Add(row);
			}

			BulkInsert(context, table);

			InsertBuyOffers(context, auctions.SelectMany(x => x.Offers));
		}

		private static void InsertMoneys(AuctioneerDbContext context, IEnumerable<Money> moneys)
		{
			var table = new DataTable("Moneys");

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

			BulkInsert(context, table);
		}

		private static void BulkInsert(AuctioneerDbContext context, DataTable data)
		{
			var bulkInsert = new SqlBulkCopy(context.Database.Connection.ConnectionString, SqlBulkCopyOptions.TableLock)
			{
				DestinationTableName = data.TableName
			};

			foreach(var column in data.Columns.Cast<DataColumn>())
			{
				bulkInsert.ColumnMappings.Add(column.ColumnName, column.ColumnName);
			}

			bulkInsert.WriteToServer(data);
		}

		private static void InsertBuyOffers(AuctioneerDbContext context, IEnumerable<BuyOffer> offers)
		{
			context.BulkInsert(offers);
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
