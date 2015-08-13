using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.EntityFramework;

namespace Auctioneer.Logic.Users
{
	public class User : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }

		public string FullName
		{
			get { return FirstName + " " + LastName; }
		}

		public string LockoutReason { get; set; }

		public bool IsBanned
		{
			get { return LockoutEndDateUtc > DateTime.UtcNow; }
		}
	}
}
