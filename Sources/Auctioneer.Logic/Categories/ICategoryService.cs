using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Categories
{
	public interface ICategoryService
	{
		Task<IEnumerable<CategoryHierarchyLevelPair>> GetAllCategoriesWithHierarchyLevel();

		// TODO auctionTitleFilter looks weird here - calculating the number of auctions in category should be
		// split into another function.
		Task<IEnumerable<Category>> GetTopLevelCategories(string auctionTitleFilter = null);
		Task<IEnumerable<Category>> GetSubcategories(int parentCategoryId, string auctionTitleFilter = null);
		Task<IEnumerable<Category>> GetCategoryHierarchy(int leafCategoryId);
	}
}