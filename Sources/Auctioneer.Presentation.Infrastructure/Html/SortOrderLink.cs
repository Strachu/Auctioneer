using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	public static class SortOrderLinkHtmlExtensions
	{
		public static IHtmlString SortOrderLink<T>(this HtmlHelper html,
		                                           string linkText,
		                                           T currentSortOrder,
		                                           T ascendingSortOrderForThisLink,
		                                           T descendingSortOrderForThisLink,
		                                           string sortOrderParam = "sortOrder",
		                                           string activeCssClass = "active",
		                                           string actionName = null,
		                                           string controllerName = null,
		                                           object routeValues = null,
		                                           object htmlAttributes = null)
		{
			var routeDictionary         = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if(!currentSortOrder.Equals(ascendingSortOrderForThisLink) &&
			   !currentSortOrder.Equals(descendingSortOrderForThisLink))
			{
				routeDictionary[sortOrderParam] = ascendingSortOrderForThisLink;

				return html.ActionLinkWithCurrentParameters(linkText, controllerName: controllerName, actionName: actionName,
				                                            routeOverrides: routeDictionary, htmlAttributes: htmlAttibutesDictionary);
			}

			var oppositeSortOrder     = descendingSortOrderForThisLink;
			var currentSortGlyphClass = "glyphicon-chevron-up";

			if(currentSortOrder.Equals(descendingSortOrderForThisLink))
			{
				oppositeSortOrder     = ascendingSortOrderForThisLink;
				currentSortGlyphClass = "glyphicon-chevron-down";				
			}

			routeDictionary[sortOrderParam] = oppositeSortOrder;

			var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
			var url       = urlHelper.ActionWithCurrentParameters(controllerName: controllerName, actionName: actionName,
			                                                      routeOverrides: routeDictionary);

			var linkBuilder = new TagBuilder("a");
			linkBuilder.Attributes.Add("href", url);
			linkBuilder.MergeAttributes(htmlAttibutesDictionary);
			linkBuilder.AddCssClass("active");
			linkBuilder.InnerHtml = String.Format("{0} <span class=\"glyphicon {1}\"></span>", linkText, currentSortGlyphClass);

			return new HtmlString(linkBuilder.ToString());
		}
	}
}