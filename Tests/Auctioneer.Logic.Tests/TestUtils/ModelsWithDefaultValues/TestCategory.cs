using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Categories;

namespace Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues
{
	internal class TestCategory : Category
	{
		private static int nextId = 1;

		public TestCategory()
		{
			Id   = nextId++;
			Name = "Not important";
		}
	}
}
