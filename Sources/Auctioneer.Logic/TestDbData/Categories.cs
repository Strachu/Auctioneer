using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Categories;

namespace Auctioneer.Logic.TestDbData
{
	internal static class Categories
	{
		public static void Add(AuctioneerDbContext context)
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

		private static void InitializeNestedSetProperties(IEnumerable<Category> categories)
		{
			int counter = 1;
			foreach(var category in categories)
			{
				InitializeNestedSetPropertiesForCategory(category, ref counter);
			}		
		}

		private static void InitializeNestedSetPropertiesForCategory(Category category, ref int counter)
		{
			category.Left = counter++;
			foreach(var subCategory in category.SubCategories)
			{
				InitializeNestedSetPropertiesForCategory(subCategory, ref counter);
			}
			category.Right = counter++;
		}
	}
}
