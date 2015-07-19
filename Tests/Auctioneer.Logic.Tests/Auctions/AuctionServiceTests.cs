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
				new Category { Id = 1 },
				new Category { Id = 2 },
			});

			context.Auctions.Add(new Auction { Title = "1", CategoryId = 1, EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "2", CategoryId = 2, EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "3", CategoryId = 2, EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "4", CategoryId = 1, EndDate = DateTime.Now.Add(TimeSpan.FromDays(1)) });
			context.Auctions.Add(new Auction { Title = "5", CategoryId = 1, EndDate = DateTime.Now.Add(TimeSpan.FromDays(2)) });
			context.Auctions.Add(new Auction { Title = "6", CategoryId = 1, EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)) });
			context.Auctions.Add(new Auction { Title = "7", CategoryId = 1, EndDate = DateTime.Now.Add(TimeSpan.FromDays(10)) });
			context.Auctions.Add(new Auction { Title = "8", CategoryId = 1, EndDate = DateTime.Now.Add(TimeSpan.FromMinutes(1)) });
			context.SaveChanges();
		}

		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsOnlyAuctionsFromSpecifiedCategory()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(2);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "2", "3" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
		
		[Test]
		public async Task GetActiveAuctionsInCategory_ReturnsOnlyAuctionsWhoseEndDateIsGreateThanCurrentDate()
		{
			var auctions = await mTestedService.GetActiveAuctionsInCategory(1);

			var returnedAuctionTitles = auctions.Select(x => x.Title);
			var expectedAuctionTitles = new string[] { "4", "5", "7", "8" };

			Assert.That(returnedAuctionTitles, Is.EquivalentTo(expectedAuctionTitles));
		}
	}
}
