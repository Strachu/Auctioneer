using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Helpers
{
	public interface IBreadcrumbBuilder
	{
		IBreadcrumbBuilder WithHomepageLink();
		IBreadcrumbBuilder WithCategoryHierarchy(int leafCategoryId);

		BreadcrumbViewModel Build();
	}
}