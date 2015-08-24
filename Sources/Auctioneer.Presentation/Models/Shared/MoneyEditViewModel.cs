using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic.Currencies;
using Auctioneer.Logic.ValueTypes;

using DataAnnotationsExtensions;

namespace Auctioneer.Presentation.Models
{
	public class MoneyEditViewModel
	{
		public MoneyEditViewModel()
		{
			Amount = 1.0m;
		}

		[Required]
		[Min(1.0)]
		public decimal Amount { get; set; }

		[Required]
		public Currency Currency { get; set; }
		public IEnumerable<SelectListItem> AvailableCurrencies { get; set; }
	}

	public static class MoneyEditViewModelExtensions
	{
		public static Money ToMoney(this MoneyEditViewModel viewModel)
		{
			return new Money(viewModel.Amount, viewModel.Currency);
		}
	}
}