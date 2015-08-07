using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slugify;

namespace Auctioneer.Presentation.Infrastructure
{
	public static class SlugGenerator
	{
		public static string SlugFromTitle(string title)
		{
			var slugGenerator = new SlugHelper();

			return slugGenerator.GenerateSlug(title);
		}
	}
}
