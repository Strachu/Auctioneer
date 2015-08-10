using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Presentation.Infrastructure.Internationalization;

using NUnit.Framework;

namespace Auctioneer.Presentation.Infrastructure.Tests.Internationalization
{
	internal class LanguageServiceTests
	{
		private LanguageService mTestedService;

		[SetUp]
		public void SetUp()
		{
			mTestedService = new LanguageService();
		}

		[Test]
		public void PolishLanguageIsSupported()
		{
			Assert.That(mTestedService.IsSupportedLanguage("pl"), Is.True);
		}

		[Test]
		public void EnglishLanguageIsSupported()
		{
			Assert.That(mTestedService.IsSupportedLanguage("en"), Is.True);
		}

		[Test]
		public void FreshLanguageIsNotSupported()
		{
			Assert.That(mTestedService.IsSupportedLanguage("fr"), Is.False);
		}

		[Test]
		public void WhenInvalidLanguageCodeIsPassed_ServiceReturnsFalse()
		{
			Assert.That(mTestedService.IsSupportedLanguage("non-existing"), Is.False);
		}
	}
}
