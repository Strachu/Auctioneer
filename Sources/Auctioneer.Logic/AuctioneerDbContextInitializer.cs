using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;

using Auctioneer.Logic.Users;

using EntityFramework.BulkInsert.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;

namespace Auctioneer.Logic
{
	internal class AuctioneerDbContextInitializer : DropCreateDatabaseIfModelChanges<AuctioneerDbContext>
	{
		protected override void Seed(AuctioneerDbContext context)
		{
			AddUsers(context);
			AddCategories(context);
			AddAuctions(context);

			base.Seed(context);
		}

		private void AddUsers(AuctioneerDbContext context)
		{
			var rndGenerator = new Random(Seed: 2934228);
			var userService  = new UserService(context, new NullUserNotifier());
			var firstNames   = new string[] { "Alexa", "Amanda", "Olivia", "Jacob", "William", "Michael", "John" };
			var lastNames    = new string[] { "Smith", "Johnson", "Williams", "Brown", "Miller", "King", "Kelly", "Foster" };
			
			for(int i = 0; i < 50; ++i)
			{
				var user = new User();

				user.FirstName      = firstNames[rndGenerator.Next(firstNames.Length)];
				user.LastName       = lastNames[rndGenerator.Next(lastNames.Length)];
				user.Address        = "ul. Wawelska 84/32\n45-345 Warszawa";
				user.Email          = user.FirstName + user.LastName + "@mail.abc";
				user.EmailConfirmed = true;
				user.UserName       = String.Format("{0}_{1}", user.FirstName, user.LastName).ToLower();

				userService.AddUser(user, "Password", new NullValidationErrorNotifier()).Wait();
			}
		}

		private void AddCategories(AuctioneerDbContext context)
		{
			var categories = new Category[]
			{
				new Category
				{
					Name          = "Computers",
					SubCategories = new Category[]
					{
						new Category { Name = "Desktop computers" },
						new Category
						{
							Name          = "Mobile computers",
							SubCategories = new Category[]
							{
								new Category { Name = "Tablets" },
								new Category { Name = "Netbooks" },
								new Category { Name = "Notebooks" },
							}
						},
						new Category
						{
							Name          = "Components",
							SubCategories = new Category[]
							{
								new Category { Name = "Hard drives" },
								new Category { Name = "Graphics cards" },
								new Category { Name = "Motherboards" },
								new Category { Name = "Processors" },
								new Category { Name = "RAM memory" },
								new Category { Name = "Power supplies" },
								new Category { Name = "Cases" },
							}
						}
					}
				},
				new Category
				{
					Name          = "Sport",
					SubCategories = new Category[]
					{
						new Category
						{
							Name          = "Cycling",
							SubCategories = new Category[]
							{
								new Category { Name = "Bicycles" },
								new Category { Name = "Accessories" },
								new Category { Name = "Clothing" }
							}
						},
						new Category
						{
							Name          = "Team sports",
							SubCategories = new Category[]
							{
								new Category { Name = "Baseball" },
								new Category { Name = "Soccer" },
								new Category { Name = "Hockey" },
							}
						},
						new Category { Name = "Weightlifting" }
					}
				},
				new Category
				{
					Name          = "Software",
					SubCategories = new Category[]
					{
						new Category
						{
							Name          = "Operating systems",
							SubCategories = new Category[]
							{
								new Category { Name = "Microsoft Windows" },
								new Category { Name = "Apple OS X" },
								new Category { Name = "Linux" },
								new Category { Name = "Other" },
							}
						},
						new Category { Name = "Office" },
						new Category { Name = "Security" },
						new Category
						{
							Name          = "Games",
							SubCategories = new Category[]
							{
								new Category { Name = "XBox One" },
								new Category { Name = "Xbox 360" },
								new Category { Name = "PlayStation 4" },
								new Category { Name = "PlayStation 3" },
								new Category { Name = "PC" },
								new Category { Name = "Other" },
							}
						},
						new Category { Name = "Programming software" },
						new Category { Name = "Other" },
					}
				}
			};

			InitializeNestedSetProperties(categories);

			context.Categories.AddRange(categories);
			context.SaveChanges();
		}

		private void InitializeNestedSetProperties(IEnumerable<Category> categories)
		{
			int counter = 1;
			foreach(var category in categories)
			{
				InitializeNestedSetPropertiesForCategory(category, ref counter);
			}		
		}

		private void InitializeNestedSetPropertiesForCategory(Category category, ref int counter)
		{
			category.Left = counter++;
			foreach(var subCategory in category.SubCategories)
			{
				InitializeNestedSetPropertiesForCategory(subCategory, ref counter);
			}
			category.Right = counter++;
		}

		private void AddAuctions(AuctioneerDbContext context)
		{
			var rndGenerator     = new Random(Seed: 746293114);
			var imageInitializer = new AuctionImageInitializer();
			var categoryCount    = context.Categories.Count();
			var userIds          = context.Users.Select(x => x.Id).ToList();

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
					Price        = rndGenerator.Next(1, 1000),
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

			context.BulkInsert(auctions);
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

		private class NullValidationErrorNotifier : IValidationErrorNotifier
		{
			public void AddError(string errorMessage)
			{
			}
		}

		private class NullUserNotifier : IUserNotifier
		{
			public Task SendActivationToken(User user, string token)
			{
				return Task.FromResult(0);
			}

			public Task SendPasswordResetToken(User user, string token)
			{
				return Task.FromResult(0);
			}

			public Task NotifyAuctionSold(User user, Auction auction)
			{
				return Task.FromResult(0);
			}

			public Task NotifyAuctionWon(User user, Auction auction)
			{
				return Task.FromResult(0);
			}
		}
	}
}
