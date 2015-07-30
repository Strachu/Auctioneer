﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Validation;

using DataAnnotationsExtensions;

using FileExtensions = System.ComponentModel.DataAnnotations.FileExtensionsAttribute;
using Lang           = Auctioneer.Presentation.Resources.Auction.Add;

namespace Auctioneer.Presentation.Models
{
	public class AuctionAddViewModel
	{
		public AuctionAddViewModel()
		{
			DaysToEnd          = 7;
			AvailableDaysToEnd = new SelectList(Enumerable.Range(1, 14));
		}

		[Required]
		[DataType(DataType.Text)]
		[StringLength(50, MinimumLength = 5)]
		[Display(Name = "AddAuction_Title", ResourceType = typeof(Lang))]
		public string Title { get; set; }

		[Required]
		[Range(1, 14)]
		[Display(Name = "AddAuction_DaysToEnd", ResourceType = typeof(Lang))]
		public int DaysToEnd { get; set; }
		public SelectList AvailableDaysToEnd { get; set; }

		[Required]
		[Min(1.0)]
		[DataType(DataType.Currency)]
		[Display(Name = "AddAuction_Price", ResourceType = typeof(Lang))]
		public decimal? Price { get; set; }

		[Required]
		[Display(Name = "AddAuction_Category", ResourceType = typeof(Lang))]
		public int CategoryId { get; set; }
		public IEnumerable<SelectListItem> AvailableCategories { get; set; }

		[Required]
		[ContentType("image/jpeg", // TODO the better solution would be to accept any image and convert it to a JPEG automatically
			ErrorMessageResourceName = "AddAuction_OnlyJPEGImagesSupported", ErrorMessageResourceType = typeof(Lang))]
		[Display(Name = "AddAuction_Photos", ResourceType = typeof(Lang))]
		public IEnumerable<HttpPostedFileBase> Photos { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Required(ErrorMessageResourceName = "AddAuction_DescriptionRequired", ErrorMessageResourceType = typeof(Lang))]
		[Display(Name = "AddAuction_Description", ResourceType = typeof(Lang))]
		public string Description { get; set; }
	}
}