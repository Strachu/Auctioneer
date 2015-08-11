using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Auctioneer.Presentation.Infrastructure.Internationalization;
using Auctioneer.Presentation.Models;

using DevTrends.MvcDonutCaching;

namespace Auctioneer.Presentation.Controllers
{
	public class LayoutController : Controller
	{
		private readonly ILanguageService mLanguageService;

		public LayoutController(ILanguageService languageService)
		{
			Contract.Requires(languageService != null);

			mLanguageService = languageService;
		}

		[ChildActionOnly]
		public PartialViewResult LoginLinks()
		{
			return (User.Identity.IsAuthenticated) ? PartialView("_LoginLinks.Authenticated")
			                                       : PartialView("_LoginLinks.NotAuthenticated");
		}

		[ChildActionOnly]
		[DonutOutputCache(Duration = Constants.DAY)]
		public PartialViewResult ChooseLanguage()
		{
			var currentRoute       = ControllerContext.ParentActionViewContext.RouteData.Values;
			var supportedLanguages = mLanguageService.GetAllLanguages().OrderBy(x => x.DisplayName);

			var viewModel = new ChooseLanguageViewModel
			{
				AvailableLanguages = supportedLanguages.Select(x =>
				{
					var routeWithNewLang     = new RouteValueDictionary(currentRoute);
					routeWithNewLang["lang"] = x.LangCode;

					return new ChooseLanguageViewModel.Language
					{
						DisplayName                  = x.DisplayName,
						CurrentPageInThisLanguageUrl = Url.RouteUrl(routeWithNewLang),
					};
				})
			};

			return PartialView("_ChooseLanguage", viewModel);
		}
	}
}