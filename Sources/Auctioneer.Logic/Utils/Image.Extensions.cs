using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Utils
{
	internal static class ImageExtensions
	{
		public static Image ScaleToFitPreservingAspectRatio(this Image imageToScale, Size destinationSize)
		{
			Contract.Requires(imageToScale != null);

			var scaleToFit = GetScaleToFit(imageToScale.Size, destinationSize);
		
			var newWidth   = (int)(imageToScale.Width  * scaleToFit);
			var newHeight  = (int)(imageToScale.Height * scaleToFit);

			return new Bitmap(imageToScale, new Size(newWidth, newHeight));
		}

		private static double GetScaleToFit(Size originalSize, Size destinationSize)
		{
			var xScale = (double)destinationSize.Width  / originalSize.Width;
			var yScale = (double)destinationSize.Height / originalSize.Height;
		
			return Math.Min(xScale, yScale);
		}
	}
}
