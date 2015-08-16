using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	public static class HtmlHelperExtensionsWithOptionalParameters
	{
		public static MvcHtmlString ActionLink(this HtmlHelper html,
		                                       string linkText,
		                                       string actionName = null,
		                                       string controllerName = null,
		                                       string areaName = null,
		                                       string protocol = null,
		                                       string hostName = null,
		                                       string fragment = null,
		                                       string cssClass = "",
		                                       object routeValues = null,
		                                       object htmlAttributes = null)
		{
			var routeDictionary         = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return ActionLink(html, linkText: linkText, areaName: areaName, actionName: actionName, controllerName: controllerName,
			                  protocol: protocol, hostName: hostName, fragment: fragment, cssClass: cssClass,
			                  routeValues: routeDictionary, htmlAttributes: htmlAttibutesDictionary);
		}

		public static MvcHtmlString ActionLink(this HtmlHelper html,
		                                       string linkText,
		                                       RouteValueDictionary routeValues,
		                                       string actionName = null,
		                                       string controllerName = null,
		                                       string areaName = null,
		                                       string protocol = null,
		                                       string hostName = null,
		                                       string fragment = null,
		                                       string cssClass = "",
		                                       IDictionary<string, object> htmlAttributes = null)
		{
			var routeDictionary         = routeValues    ?? new RouteValueDictionary();
			var htmlAttibutesDictionary = htmlAttributes ?? new Dictionary<string, object>();

			if(!String.IsNullOrWhiteSpace(cssClass))
			{
				htmlAttibutesDictionary["class"] = cssClass;
			}

			if(areaName != null)
			{
				routeDictionary["area"] = areaName;
			}

			return LinkExtensions.ActionLink(html, linkText: linkText, actionName: actionName, controllerName: controllerName,
			                                 protocol: protocol, hostName: hostName, fragment: fragment,
			                                 routeValues: routeDictionary, htmlAttributes: htmlAttibutesDictionary);
		}

		public static MvcForm BeginForm(this HtmlHelper html,
		                                string controllerName = null,
		                                string actionName = null,
		                                object routeValues = null,
		                                FormMethod method = FormMethod.Post,
		                                string cssClass = "",
		                                object htmlAttributes = null)
		{
			var routeDictionary         = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return html.BeginForm(controllerName: controllerName, actionName: actionName, routeValues: routeDictionary,
			                      method: method, cssClass: cssClass, htmlAttributes: htmlAttibutesDictionary);
		}

		public static MvcForm BeginForm(this HtmlHelper html,
		                                RouteValueDictionary routeValues,
		                                string controllerName = null,
		                                string actionName = null,
		                                FormMethod method = FormMethod.Post,
		                                string cssClass = "",
		                                IDictionary<string, object> htmlAttributes = null)
		{
			var htmlAttibutesDictionary = htmlAttributes ?? new Dictionary<string, object>();

			if(!String.IsNullOrWhiteSpace(cssClass))
			{
				htmlAttibutesDictionary["class"] = cssClass;
			}

			return FormExtensions.BeginForm(html, controllerName: controllerName, actionName: actionName,
			                                routeValues: routeValues, method: method, htmlAttributes: htmlAttibutesDictionary);
		}

		public static MvcHtmlString ValidationSummary(this HtmlHelper html,
		                                              bool excludePropertyErrors = false,
		                                              string message = "",
		                                              object htmlAttributes = null,
		                                              string headingTag = null)
		{
			return ValidationExtensions.ValidationSummary(html, excludePropertyErrors: excludePropertyErrors, message: message,
			                                              htmlAttributes: htmlAttributes, headingTag: headingTag);
		}

		public static void RenderAction(this HtmlHelper html,
		                                string areaName = null,
		                                string actionName = null,
		                                string controllerName = null,
		                                object routeValues = null,
		                                bool excludeFromParentCache = false)
		{
			var routeDictionary = new RouteValueDictionary(routeValues);

			if(areaName != null)
			{
				routeDictionary["area"] = areaName;
			}

			DevTrends.MvcDonutCaching.HtmlHelperExtensions.RenderAction(html, actionName: actionName, 
			                                                            controllerName: controllerName,
			                                                            routeValues: routeDictionary,
			                                                            excludeFromParentCache: excludeFromParentCache);
		}
	}
}