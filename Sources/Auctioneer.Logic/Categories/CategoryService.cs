using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Categories
{
	public class CategoryService : ICategoryService
	{
		private readonly AuctioneerDbContext mContext;

		public CategoryService(AuctioneerDbContext context)
		{
			mContext = context;
		}

		public Task<IEnumerable<Category>> GetTopLevelCategories()
		{
			return GetCategoriesAlongWithAuctionCount(x => x.ParentId == null);
		}

		public Task<IEnumerable<Category>> GetSubcategories(int parentCategoryId)
		{
			return GetCategoriesAlongWithAuctionCount(x => x.ParentId == parentCategoryId);
		}

		private async Task<IEnumerable<Category>> GetCategoriesAlongWithAuctionCount(
			Expression<Func<Category, bool>> categoryFilter)
		{
			var categories = mContext.Categories.Where(categoryFilter);

			var query = from rootCategory in categories
			            from subCategory  in mContext.Categories
			            where subCategory.Left  >= rootCategory.Left &&
			                  subCategory.Right <= rootCategory.Right

			            join auction in mContext.Auctions
			            on subCategory.Id equals auction.CategoryId into outerJoin
			            from auction in outerJoin.Where(x => x.EndDate > DateTime.Now).DefaultIfEmpty()

			            group auction by rootCategory into auctionByRootCategory
			            orderby auctionByRootCategory.Key.Name ascending
			            select new
			            {
			               Category     = auctionByRootCategory.Key,
			               AuctionCount = auctionByRootCategory.Count(x => x != null)
			            };			

			var categoriesWithAuctionCount = await query.ToListAsync().ConfigureAwait(false);

			return categoriesWithAuctionCount.Select(x =>
			{
				var category = x.Category;

				category.AuctionCount = x.AuctionCount;
				return category;
			});
		}
	}
}
