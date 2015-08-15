using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Currencies;

namespace Auctioneer.Presentation.Models.Shared
{
	public class MoneyEditViewModel
	{
		public decimal Amount { get; set; }
		public Currency Currency { get; set; }
		public IEnumerable<SelectListItem> AvailableCurrencies { get; set; }
	}
}