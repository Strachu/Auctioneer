using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Presentation.Helpers
{
	[ContractClassFor(typeof(IBreadcrumbBuilder))]
	internal abstract class IBreadcrumbBuilderContractClass : IBreadcrumbBuilder
	{
		IBreadcrumbBuilder IBreadcrumbBuilder.WithHomepageLink()
		{
			throw new NotImplementedException();
		}

		IBreadcrumbBuilder IBreadcrumbBuilder.WithCategoryHierarchy(int leafCategoryId)
		{
			throw new NotImplementedException();
		}

		IBreadcrumbBuilder IBreadcrumbBuilder.WithAuctionLink(Logic.Auctions.Auction auction)
		{
			Contract.Requires(auction != null);

			throw new NotImplementedException();
		}

		Models.BreadcrumbViewModel IBreadcrumbBuilder.Build()
		{
			throw new NotImplementedException();
		}
	}
}
