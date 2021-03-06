﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Validation;

using Lang = Auctioneer.Resources.Auction.Add;

namespace Auctioneer.Presentation.Models
{
	public class AuctionAddViewModel
	{
		public AuctionAddViewModel()
		{
			DaysToEnd           = 7;
			AvailableDaysToEnd  = new SelectList(Enumerable.Range(1, 14));
			MinimumBiddingPrice = new MoneyEditViewModel();
			BuyoutPrice         = new MoneyEditViewModel();
		}

		[Required]
		[DataType(DataType.Text)]
		[StringLength(50, MinimumLength = 5)]
		[Display(Name = "Title", ResourceType = typeof(Lang))]
		public string Title { get; set; }

		[Required]
		[Range(1, 14)]
		[Display(Name = "DaysToEnd", ResourceType = typeof(Lang))]
		public int DaysToEnd { get; set; }
		public SelectList AvailableDaysToEnd { get; set; }

		[Display(Name = "EnableBidding", ResourceType = typeof(Lang))]
		[AtLeastOneEnabled("Price", ErrorMessageResourceName = "BiddingOrCheckoutRequired", ErrorMessageResourceType = typeof(Lang))]
		public bool IsBiddingEnabled { get; set; }

		[Required]
	//	[Min(1.0)] TODO add validation support to MoneyEditViewModel
		[DataType(DataType.Currency)]
		[Display(Name = "MinimumPrice", ResourceType = typeof(Lang))]
		public MoneyEditViewModel MinimumBiddingPrice { get; set; }

		[Display(Name = "EnableBuyout", ResourceType = typeof(Lang))]
		[AtLeastOneEnabled("Price", ErrorMessageResourceName = "BiddingOrCheckoutRequired", ErrorMessageResourceType = typeof(Lang))]
		public bool IsBuyoutEnabled { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Display(Name = "Price", ResourceType = typeof(Lang))]
		public MoneyEditViewModel BuyoutPrice { get; set; }

		[Required]
		[Display(Name = "Category", ResourceType = typeof(Lang))]
		public int CategoryId { get; set; }
		public IEnumerable<SelectListItem> AvailableCategories { get; set; }

		[Required]
		[ContentType("image/jpeg", // TODO the better solution would be to accept any image and convert it to a JPEG automatically
			ErrorMessageResourceName = "OnlyJPEGImagesSupported", ErrorMessageResourceType = typeof(Lang))]
		[Display(Name = "Photos", ResourceType = typeof(Lang))]
		public IEnumerable<HttpPostedFileBase> Photos { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Required(ErrorMessageResourceName = "DescriptionRequired", ErrorMessageResourceType = typeof(Lang))]
		[Display(Name = "Description", ResourceType = typeof(Lang))]
		public string Description { get; set; }
	}
}