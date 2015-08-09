using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Auctions;

namespace Auctioneer.Logic.Categories
{
	public class CategoryService : ICategoryService
	{
		private readonly AuctioneerDbContext mContext;

		public CategoryService(AuctioneerDbContext context)
		{
			mContext = context;
		}

		public async Task<IEnumerable<CategoryHierarchyLevelPair>> GetAllCategoriesWithHierarchyLevel()
		{
			var query = from category       in mContext.Categories
			            from parentCategory in mContext.Categories
			            where category.Left  >= parentCategory.Left &&
			                  category.Right <= parentCategory.Right

			            group parentCategory by category into grouping
			            orderby grouping.Key.Left ascending

			            select new CategoryHierarchyLevelPair
			            {
				            Category       = grouping.Key,
				            HierarchyDepth = grouping.Count() - 1
			            };

			return await query.ToListAsync();
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
			var query = from rootCategory in mContext.Categories.Where(categoryFilter)
			            from subCategory  in mContext.Categories
			            where subCategory.Left  >= rootCategory.Left &&
			                  subCategory.Right <= rootCategory.Right

			            join auction in mContext.Auctions.Where(AuctionStatusFilter.Active)
			            on subCategory.Id equals auction.CategoryId into outerJoin
			            from auction in outerJoin.DefaultIfEmpty()

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

		public async Task<IEnumerable<Category>> GetCategoryHierarchy(int leafCategoryId)
		{
			var query = from leafCategory in mContext.Categories
			            where leafCategory.Id == leafCategoryId

			            from categoryInHierarchy in mContext.Categories
			            where categoryInHierarchy.Left <= leafCategory.Left && categoryInHierarchy.Right >= leafCategory.Right
			            select categoryInHierarchy;

			return await query.ToListAsync().ConfigureAwait(false);
		}
	}
}
