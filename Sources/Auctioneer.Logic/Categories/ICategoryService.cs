using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Categories
{
	public interface ICategoryService
	{
		Task<IEnumerable<Category>> GetTopLevelCategories();
		Task<IEnumerable<Category>> GetSubcategories(int parentCategoryId);
	}
}