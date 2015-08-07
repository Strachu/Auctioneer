using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Auctioneer.Presentation.Infrastructure.Validation;

using FakeItEasy;

using NUnit.Framework;

namespace Auctioneer.Presentation.Infrastructure.Tests.Validation
{
	internal class ContentTypeAttributesTests
	{
		[Test]
		public void WhenSingleFileIsTestedAndItHasCorrectContentType_ValidationIsSuccessful()
		{
			var file = CreateFileWithContentType("image/jpeg");

			var testedAttribute  = new ContentTypeAttribute("image/jpeg");
			var validationResult = testedAttribute.GetValidationResult(file, new ValidationContext(file));

			Assert.That(validationResult, Is.Null);
		}

		[Test]
		public void WhenSingleFileIsTestedAndItHasIncorrectContentType_ValidationReturnsError()
		{
			var file = CreateFileWithContentType("image/png");

			var testedAttribute  = new ContentTypeAttribute("image/jpeg");
			var validationResult = testedAttribute.GetValidationResult(file, new ValidationContext(file));

			Assert.That(validationResult, Is.Not.Null);
		}

		[Test]
		public void WhenMultipleFilesAreTestedAndEveryFileHasCorrectContentType_ValidationIsSuccessful()
		{
			var files = new HttpPostedFileBase[]
			{
				CreateFileWithContentType("image/jpeg"),
				CreateFileWithContentType("image/jpeg"),
				CreateFileWithContentType("image/jpeg"),
			};

			var testedAttribute  = new ContentTypeAttribute("image/jpeg");
			var validationResult = testedAttribute.GetValidationResult(files, new ValidationContext(files));

			Assert.That(validationResult, Is.Null);
		}

		[Test]
		public void WhenMultipleFilesAreTestedAndSomeFilesHasIncorrectContentType_ValidationReturnsError()
		{
			var files = new HttpPostedFileBase[]
			{
				CreateFileWithContentType("image/jpeg"),
				CreateFileWithContentType("image/png"),
				CreateFileWithContentType("image/jpeg"),
			};

			var testedAttribute  = new ContentTypeAttribute("image/jpeg");
			var validationResult = testedAttribute.GetValidationResult(files, new ValidationContext(files));

			Assert.That(validationResult, Is.Not.Null);
		}

		private HttpPostedFileBase CreateFileWithContentType(string contentType)
		{
			var file = A.Fake<HttpPostedFileBase>();
			A.CallTo(() => file.ContentType).Returns(contentType);	
	
			return file;
		}
	}
}
