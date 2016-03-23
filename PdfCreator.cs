using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Splendor
{
	public static class PdfCreator
	{
		public static PdfDocument CreatePdfDocument(IList<Image> images)
		{
			const int pageWidth = 816*72/96;
			const int pageHeight = 1056*72/96;
			var firstXImage = XImage.FromGdiPlusImage(images.First());
			var horizontalWhiteSpace = (pageWidth - (3*firstXImage.PointWidth))/4;
			var verticalWhiteSpace = (pageHeight - (3*firstXImage.PointHeight))/4;
			var document = new PdfDocument();
			var remainingImages = images.ToList();
			while (remainingImages.Any())
			{
				var nextNine = remainingImages.Take(9).ToList();
				var page = document.AddPage();
				page.Size = PageSize.Letter;
				var xGraphics = XGraphics.FromPdfPage(page);

				for (var index = 0; index < 9 && index < nextNine.Count; index++)
				{
					var xImage = XImage.FromGdiPlusImage(nextNine[index]);

					var row = index % 3;
					var column = index / 3;

					var x = (row * (xImage.PointWidth + horizontalWhiteSpace)) + horizontalWhiteSpace;
					var y = (column * (xImage.PointHeight + verticalWhiteSpace)) + verticalWhiteSpace;

					xGraphics.DrawImage(xImage, x, y);
				}

				remainingImages = remainingImages.Skip(9).ToList();
			}
			return document;
		}
	}
}
