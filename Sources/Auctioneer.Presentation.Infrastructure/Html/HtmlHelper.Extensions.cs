﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using Auctioneer.Presentation.Infrastructure.Http;

namespace Auctioneer.Presentation.Infrastructure.Html
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
			var routeDictionary         = HttpContext.Current.Request.QueryString.ToRouteDictionary();
			var routeOverrides          = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

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

		public static IHtmlString EnumDropDownList(this HtmlHelper html,
		                                           string name,
		                                           Enum value,
		                                           object htmlAttributes = null)
		{
			// EnumHelper.GetSelectList does not work with [Flags].
			var selectList = EnumSelectList.FromSelectedValue(value);

			return html.DropDownList(name, selectList, htmlAttributes);
		}

		public static IHtmlString CheckBox<T>(this HtmlHelper html,
		                                      string name,
		                                      T value,
		                                      bool disabled = false)
		{
			var builder = new TagBuilder("input");
			builder.Attributes.Add("type", "checkbox");
			builder.Attributes.Add("name", name);
			builder.Attributes.Add("id",   name);
			builder.Attributes.Add("value", value.ToString());
			if(disabled)
			{
				builder.Attributes.Add("disabled", "true");
			}

			return new HtmlString(builder.ToString());
		}
	}
}