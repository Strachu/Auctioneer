using System;
using System.Collections.Generic;
using System.Data.Entity;
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
			AddCategories(context);
			AddAuctions(context);

			base.Seed(context);
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
			var rndGenerator  = new Random(Seed: 746293114);
			var categoryCount = context.Categories.Count();

			var auctions = new Auction[1000000];
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

				auctions[i] = new Auction
				{
					CategoryId   = rndGenerator.Next(categoryCount) + 1,
					Title        = "The auction #" + (i + 1),
					Description  = "The description of auction number " + i,
					CreationDate = creationDate,
					EndDate      = creationDate.AddDays(rndGenerator.NextDouble() * 14),
					Price        = rndGenerator.Next(1, 1000),
				};
			}

			context.BulkInsert(auctions);
		}
	}
}
