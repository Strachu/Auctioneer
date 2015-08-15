using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Presentation.Infrastructure.Html;

using FakeItEasy;

using NUnit.Framework;

namespace Auctioneer.Presentation.Infrastructure.Tests.Html
{
	internal class HtmlHelperExtensionsTests
	{
		private HtmlHelper mHtmlHelper;

		[SetUp]
		public void SetUp()
		{
			mHtmlHelper = new HtmlHelper(new ViewContext(), A.Fake<IViewDataContainer>());
		}

		[Test]
		public void RenderAttributesReturnsCorrectlyFormattedAttributesFromSingleObject()
		{
			var result = mHtmlHelper.RenderAttributes(new
			{
				disabled = true,
				@class   = "class1 class2",
				id       = "value"
			});

			Assert.That(result.ToHtmlString(), Is.EqualTo("disabled=\"true\" class=\"class1 class2\" id=\"value\"").IgnoreCase);
		}

		[Test]
		public void RenderAttributesReturnsCorrectlyFormattedAttributesFromTwoObjectWhenAttributesDoesNotCollide()
		{
			var result = mHtmlHelper.RenderAttributes(new
			{
				disabled = true,
				@class   = "class1 class2",
			},
			new
			{
				id       = "value",
				style    = "width: 10px; border: 0"
			});

			Assert.That(result.ToHtmlString(), Is.EqualTo("disabled=\"true\" class=\"class1 class2\" id=\"value\"" + 
			                                              " style=\"width: 10px; border: 0\"").IgnoreCase);
		}

		[Test]
		public void WhenCssClassIsSpecifiedInBothObjects_RenderAttributesMergesIt()
		{
			var result = mHtmlHelper.RenderAttributes(new
			{
				disabled = true,
				@class   = "class1",
			},
			new
			{
				@class   = "class2",
				id       = "value",
			});

			Assert.That(result.ToHtmlString(), Is.EqualTo("disabled=\"true\" class=\"class1 class2\" id=\"value\"").IgnoreCase);
		}
	}
}
