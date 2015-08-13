using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Auctioneer.Presentation.Infrastructure.Formatting;

using Lang = Auctioneer.Resources.Other;

namespace Auctioneer.Presentation.Infrastructure.Html
{
	internal enum DurationUnit
	{
		Days,
		Hours,
		Minutes
	}

	public static class DurationEditorHtmlExtensions
	{
		public static IHtmlString DurationEditor(this HtmlHelper<TimeSpan> html,
		                                         TimeSpan currentValue,
		                                         object htmlAttributes = null)
		{
			var htmlAttibutesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			var currentValueParsed = TimeSpanFormatter.Format(currentValue).Split(' ');
			var value              = currentValueParsed[0];
			var currentUnit        = currentValueParsed[1];

			var availableUnits = new SelectListItem[]
			{
				new SelectListItem { Text = Lang.TimeSpan_Days,    Value = DurationUnit.Days.ToString() },
				new SelectListItem { Text = Lang.TimeSpan_Hours,   Value = DurationUnit.Hours.ToString() },
				new SelectListItem { Text = Lang.TimeSpan_Minutes, Value = DurationUnit.Minutes.ToString() },
			};

			foreach(var unit in availableUnits)
			{
				unit.Selected = (unit.Text == currentUnit);
			}

			var textbox  = html.NumberInput(String.Empty, value: value, min: 1, cssClass: "value");
			var dropdown = html.DropDownList("Unit", availableUnits, htmlAttributes: new { @class = "unit" });

			var div = new TagBuilder("div");
			div.MergeAttributes(htmlAttibutesDictionary);
			div.AddCssClass("duration-editor");
			div.InnerHtml = textbox.ToHtmlString() + dropdown.ToHtmlString();

			return new HtmlString(div.ToString());
		}
	}
}
