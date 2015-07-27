using System.Diagnostics.Contracts;

using Auctioneer.Logic.Auctions;
using Auctioneer.Presentation.Models;

namespace Auctioneer.Presentation.Helpers
{
	[ContractClass(typeof(IBreadcrumbBuilderContractClass))]
	public interface IBreadcrumbBuilder
	{
		IBreadcrumbBuilder WithHomepageLink();
		IBreadcrumbBuilder WithCategoryHierarchy(int leafCategoryId);
		IBreadcrumbBuilder WithAuctionLink(Auction auction);

		BreadcrumbViewModel Build();
	}
}