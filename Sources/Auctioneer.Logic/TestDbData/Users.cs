using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Users;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Auctioneer.Logic.TestDbData
{
	internal static class Users
	{
		public static void Add(AuctioneerDbContext context)
		{
			var rndGenerator = new Random(Seed: 2934228);
			var userManager  = new UserManager<User>(new UserStore<User>(context));
			var roleManager  = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
			var firstNames   = new string[] { "Alexa", "Amanda", "Olivia", "Jacob", "William", "Michael", "John" };
			var lastNames    = new string[] { "Smith", "Johnson", "Williams", "Brown", "Miller", "King", "Kelly", "Foster" };
			
			roleManager.Create(new IdentityRole { Name = "Admin" });

			for(int i = 0; i < 50; ++i)
			{
				var user = new User();

				user.FirstName      = firstNames[rndGenerator.Next(firstNames.Length)];
				user.LastName       = lastNames[rndGenerator.Next(lastNames.Length)];
				user.Address        = "ul. Wawelska 84/32\n45-345 Warszawa";
				user.Email          = user.FirstName + user.LastName + "@mail.abc";
				user.EmailConfirmed = true;
				user.UserName       = String.Format("{0}_{1}", user.FirstName, user.LastName).ToLower();

				userManager.Create(user, "Password");
			}

			var admin = new User
			{
				UserName       = "Admin",
				FirstName      = "Mr. Admin",
				LastName       = "Admin",
				Address        = "Administrator Panel",
				Email          = "admin@admins.com",
				EmailConfirmed = true,
			};

			userManager.Create(admin, password: "Admin");
			userManager.AddToRole(admin.Id, "Admin");
		}
	}
}
