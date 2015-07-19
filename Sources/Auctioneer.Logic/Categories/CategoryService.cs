using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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

		public async Task<IEnumerable<Category>> GetTopLevelCategories()
		{
			return await mContext.Categories.Where(x => x.Parent == null).ToListAsync();
		}

		public Task<Category> GetCategoryById(int categoryId)
		{
			return mContext.Categories.FindAsync(categoryId);
		}
	}
}
