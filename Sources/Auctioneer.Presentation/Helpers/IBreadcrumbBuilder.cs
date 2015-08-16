using System.Diagnostics.Contracts;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Helpers
{
	[ContractClass(typeof(IBreadcrumbBuilderContractClass))]
	public interface IBreadcrumbBuilder
	{
		IBreadcrumbBuilder WithHomepageLink();
		IBreadcrumbBuilder WithCategoryHierarchy(int leafCategoryId, string searchString = null);
		IBreadcrumbBuilder WithAuctionLink(Auction auction);
		IBreadcrumbBuilder WithSearchResults(string searchString);

		BreadcrumbViewModel Build();
	}
}