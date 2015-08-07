using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Users;

namespace Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues
{
	internal class TestUser : User
	{
		private static int nextId = 1;

		public TestUser()
		{
			base.Id        = nextId.ToString();
			base.UserName  = nextId.ToString();
			base.Email     = String.Format("{0}@.com", nextId);
			base.Address   = "Address";
			base.FirstName = "FirstName";
			base.LastName  = "LastName";

			nextId++;
		}
	}
}
