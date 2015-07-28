using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Categories
{
	public interface ICategoryService
	{
		Task<IEnumerable<CategoryHierarchyLevelPair>> GetAllCategoriesWithHierarchyLevel();

		Task<IEnumerable<Category>> GetTopLevelCategories();
		Task<IEnumerable<Category>> GetSubcategories(int parentCategoryId);
		Task<IEnumerable<Category>> GetCategoryHierarchy(int leafCategoryId);
	}
}