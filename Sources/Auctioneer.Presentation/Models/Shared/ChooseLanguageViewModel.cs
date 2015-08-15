using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Models
{
	public class ChooseLanguageViewModel
	{
		public IEnumerable<Language> AvailableLanguages;

		public class Language
		{
			public string DisplayName { get; set; }
			public string CurrentPageInThisLanguageUrl { get; set; }
		}
	}
}