using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Categories
{
	public interface ICategoryService
	{
		Task<IEnumerable<CategoryHierarchyLevelPair>> GetAllCategoriesWithHierarchyLevel();

		Task<IEnumerable<CategoryAuctionCountPair>> GetCategoriesAlongWithAuctionCount(int? parentCategoryId = null,
		                                                                               string auctionTitleFilter = null);
	
		Task<IEnumerable<Category>> GetCategoryHierarchy(int leafCategoryId);
	}
}