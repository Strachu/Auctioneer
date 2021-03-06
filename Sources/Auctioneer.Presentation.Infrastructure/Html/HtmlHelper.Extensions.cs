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
		                                             string protocol = null,
		                                             string hostName = null,
		                                             string fragment = null,
		                                             string cssClass = "",
		                                             object routeValues = null,
		                                             object htmlAttributes = null)
		{
			var routeDictionary          = new RouteValueDictionary(routeValues);
			var htmlAttributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			routeDictionary["slug"] = SlugGenerator.SlugFromTitle(linkText);

			return helper.ActionLink(linkText: linkText, actionName: actionName, controllerName: controllerName,
			                         protocol: protocol, hostName: hostName, fragment: fragment, cssClass: cssClass,
			                         routeValues: routeDictionary, htmlAttributes: htmlAttributesDictionary);
		}

		public static IHtmlString ActionLinkWithCurrentParameters(this HtmlHelper helper,
		                                                          string linkText,
		                                                          string actionName = null,
		                                                          string controllerName = null,
		                                                          object routeValues = null,
		                                                          object htmlAttributes = null)
		{
			var routeDictionary         = new RouteValueDictionary(routeValues);
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return helper.ActionLinkWithCurrentParameters(linkText, actionName: actionName, controllerName: controllerName,
			                                              routeOverrides: routeDictionary,
			                                              htmlAttributes: htmlAttibutesDictionary);
		}

		public static IHtmlString ActionLinkWithCurrentParameters(this HtmlHelper helper,
		                                                          string linkText,
		                                                          RouteValueDictionary routeOverrides,
		                                                          string actionName = null,
		                                                          string controllerName = null,
		                                                          IDictionary<string, object> htmlAttributes = null)
		{
			var routeDictionary = HttpContext.Current.Request.QueryString.ToRouteDictionary();

			foreach(var routeOverride in routeOverrides)
			{
				routeDictionary[routeOverride.Key] = routeOverride.Value;
			}

			return helper.ActionLink(linkText, actionName, controllerName, routeDictionary, htmlAttributes);
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
		                                           bool multiple = false,
		                                           string cssClass = "",
		                                           object htmlAttributes = null)
		{
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if(!String.IsNullOrWhiteSpace(cssClass))
			{
				htmlAttibutesDictionary["class"] = cssClass;
			}

			if(multiple)
			{
				htmlAttibutesDictionary["multiple"] = true;
			}

			// EnumHelper.GetSelectList does not work with [Flags].
			var selectList = EnumSelectList.FromSelectedValue(value);

			return html.DropDownList(name, selectList, htmlAttibutesDictionary);
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

		public static IHtmlString NumberInputFor<TModel, TProperty>(this HtmlHelper<TModel> html,
		                                                            Expression<Func<TModel, TProperty>> expression,
		                                                            string cssClass = "",
		                                                            object min = null,
		                                                            object max = null,
		                                                            object step = null,
		                                                            object htmlAttributes = null)
		{
			var name  = ExpressionHelper.GetExpressionText(expression);
			var value = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model;

			return html.NumberInput(name, value, cssClass: cssClass, min: min, max: max, step: step,
			                        htmlAttributes: htmlAttributes);
		}

		public static IHtmlString NumberInput<T>(this HtmlHelper html,
		                                         string name,
		                                         T value,
		                                         string cssClass = "",
		                                         object min = null,
		                                         object max = null,
		                                         object step = null,
		                                         object htmlAttributes = null)
		{
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return html.NumberInput(name, value: value, cssClass: cssClass, min: min, max: max, step: step,
			                        htmlAttibutes: htmlAttibutesDictionary);
		}

		public static IHtmlString NumberInput<T>(this HtmlHelper html,
		                                         string name,
		                                         T value,
		                                         IDictionary<string, object> htmlAttibutes,
		                                         string cssClass = "",
		                                         object min = null,
		                                         object max = null,
		                                         object step = null)
		{
			htmlAttibutes = htmlAttibutes ?? new Dictionary<string, object>();

			htmlAttibutes["type"] = "number";

			if(!String.IsNullOrWhiteSpace(cssClass))
			{
				htmlAttibutes["class"] = cssClass;
			}

			if(min != null)
			{
				htmlAttibutes["min"] = min;
			}

			if(max != null)
			{
				htmlAttibutes["max"] = max;
			}

			if(step != null)
			{
				htmlAttibutes["step"] = step;
			}

			return html.TextBox(name, value: value, htmlAttributes: htmlAttibutes);
		}

		public static IHtmlString SearchBox(this HtmlHelper html,
		                                    string name,
		                                    object value,
		                                    string cssClass = "",
		                                    string placeholder = "",
		                                    object htmlAttributes = null)
		{
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			htmlAttibutesDictionary["type"] = "search";

			if(!String.IsNullOrWhiteSpace(cssClass))
			{
				htmlAttibutesDictionary["class"] = cssClass;
			}

			if(!String.IsNullOrWhiteSpace(placeholder))
			{
				htmlAttibutesDictionary["placeHolder"] = placeholder;
			}

			return html.TextBox(name, value: value, htmlAttributes: htmlAttibutesDictionary);
		}

		public static IHtmlString RenderAttributes(this HtmlHelper html,
		                                           object htmlAttributes,
		                                           object htmlAttributesOverrides = null)
		{
			var htmlAttributesDictionary          = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			var htmlAttributesOverridesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesOverrides);

			foreach(var attribute in htmlAttributesOverridesDictionary)
			{
				if(attribute.Key == "class")
				{
					htmlAttributesDictionary[attribute.Key] += " " + attribute.Value;
				}
				else
				{
					htmlAttributesDictionary[attribute.Key] = attribute.Value;
				}
			}

			var attributesWithValues = htmlAttributesDictionary.Select(x => String.Format("{0}=\"{1}\"", x.Key, x.Value));
			return new HtmlString(String.Join(" ", attributesWithValues));
		}
	}
}