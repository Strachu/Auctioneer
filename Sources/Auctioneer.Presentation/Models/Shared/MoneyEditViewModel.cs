using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Currencies;

using DataAnnotationsExtensions;

namespace Auctioneer.Presentation.Models.Shared
{
	public class MoneyEditViewModel
	{
		[Required]
		[Min(1.0)]
		public decimal Amount { get; set; }

		[Required]
		public Currency Currency { get; set; }
		public IEnumerable<SelectListItem> AvailableCurrencies { get; set; }
	}
}