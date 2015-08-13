using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Lang = Auctioneer.Resources.User.Lockout;

namespace Auctioneer.Presentation.Models.User
{
	public class UserLockoutViewModel
	{
		[DataType(DataType.DateTime)]
		public DateTime ExpiryDate { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan TimeLeft
		{
			get { return ExpiryDate.Subtract(DateTime.Now); }
		}

		[DataType(DataType.MultilineText)]
		public string Reason { get; set; }
	}
}