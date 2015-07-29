using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Utils;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Utils
{
	internal class ImageExtensionsTests
	{
		[Test]
		public void WhenImageWidthIsBiggerThanItsHeight_ScaleToFitPreservesAspectRatio()
		{
			var originalImage = new Bitmap(1000, 100, PixelFormat.Format24bppRgb);

			var scaledImage = originalImage.ScaleToFitPreservingAspectRatio(new Size(200, 50));
		
			Assert.That(scaledImage.Size, Is.EqualTo(new Size(200, 20)));
		}
	
		[Test]
		public void WhenImageHeightIsBiggerThanItsWidth_ScaleToFitPreservesAspectRatio()
		{
			var originalImage = new Bitmap(200, 500, PixelFormat.Format24bppRgb);
		
			var scaledImage = originalImage.ScaleToFitPreservingAspectRatio(new Size(150, 300));
		
			Assert.That(scaledImage.Size, Is.EqualTo(new Size(120, 300)));
		}
	}
}
