using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Auctioneer.Logic.Utils;

namespace Auctioneer.Presentation.Helpers
{
	public static class HtmlHelperExtensions
	{
		public static IHtmlString ActionLinkWithSlug(this HtmlHelper helper,
		                                             string linkText,
		                                             string actionName,
		                                             string controllerName = null,
		                                             object routeValues = null,
		                                             object htmlAttributes = null)
		{
			var routeDictionary         = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			routeDictionary["slug"] = SlugGenerator.SlugFromTitle(linkText);

			return helper.ActionLink(linkText, actionName, controllerName, routeDictionary, htmlAttibutesDictionary);
		}

		public static IHtmlString ActionLinkWithCurrentParameters(this HtmlHelper helper,
		                                                          string linkText,
		                                                          string actionName,
		                                                          string controllerName = null,
		                                                          object routeValues = null,
		                                                          object htmlAttributes = null)
		{
			var currentParameters       = HttpContext.Current.Request.QueryString;
			var routeDictionary         = new RouteValueDictionary();
			var routeOverrides          = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			foreach(var parameter in currentParameters.AllKeys)
			{
				routeDictionary[parameter] = currentParameters[parameter];
			}

			foreach(var routeOverride in routeOverrides)
			{
				routeDictionary[routeOverride.Key] = routeOverride.Value;
			}

			return helper.ActionLink(linkText, actionName, controllerName, routeDictionary, htmlAttibutesDictionary);
		}

		public static IHtmlString FileUploadFor<TModel, TProperty>(this HtmlHelper<TModel> html,
		                                                           Expression<Func<TModel, TProperty>> expression,
		                                                           string acceptFilter)
		{
			Contract.Requires(html != null);
			Contract.Requires(expression != null);

			bool allowMultiple = typeof(IEnumerable).IsAssignableFrom(expression.ReturnType);

			return html.TextBoxFor(expression, htmlAttributes: new
			{
				type     = "file",
				accept   = acceptFilter,
				multiple = allowMultiple
			});
		}
	}
}