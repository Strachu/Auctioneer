using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;
using Auctioneer.Logic.Categories;

using Effort.Provider;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Auctions
{
	internal class AuctionServiceTests
	{
		private AuctionService mTestedService;

		[SetUp]
		public void SetUp()
		{
			var context = new AuctioneerDbContext(Effort.DbConnectionFactory.CreateTransient());

			AddTestData(context);

			mTestedService = new AuctionService(context);
		}

		private void AddTestData(AuctioneerDbContext context)
		{
			context.Categories.AddRange(new Category[]
			{
				new Category
				{
					Id = 1, Left = 1, Right = 10,
					SubCategories = new Category[]
					{
						new Category { Id = 2, Left = 2, Right = 3 },		
						new Category
						{
							Id = 3, Left = 4, Right = 9,						
							SubCategories = new Category[]
							{
								new Category { Id = 4, Left = 5, Right = 6 },		
								new Category { Id = 5, Left = 7, Right = 8 },					
							}	
						},				

					}
				},
				new Category { Id = 6, Left = 11, Right = 12 },
			});

			context.Auctions.Add(new Auction { Title = "1",  CategoryId = 2, EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "2",  CategoryId = 6, EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "3",  CategoryId = 6, EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "4",  CategoryId = 2, EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)) });
			context.Auctions.Add(new Auction { Title = "5",  CategoryId = 2, EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "6",  CategoryId = 2, EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)) });
			context.Auctions.Add(new Auction { Title = "7",  CategoryId = 2, EndDate = DateTime.Now.Add(TimeSpan.FromDays(10)) });
			context.Auctions.Add(new Auction { Title = "8",  CategoryId = 2, EndDate = DateTime.Now.Add(TimeSpan.FromMinutes(1)) });
			context.Auctions.Add(new Auction { Title = "9",  CategoryId = 3, EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)) });
			context.Auctions.Add(new Auction { Title = "10", CategoryId = 5, EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)) });
			context.SaveChanges();
		}

		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsOnlyAuctionsFromSpecifiedCategory()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(6);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "2", "3" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsOnlyAuctionsWhoseEndDateIsGreateThanCurrentDate()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(2);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "4", "5", "7", "8" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsAlsoAuctionsFromSubcategories()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(1);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "4", "5", "7", "8", "9", "10" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
	}
}
