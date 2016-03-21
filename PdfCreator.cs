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

					var x = (row * (xImage.PointWidth + 15)) + 15;
					var y = (column * (xImage.PointHeight + 8)) + 8;

					xGraphics.DrawImage(xImage, x, y);
				}

				remainingImages = remainingImages.Skip(9).ToList();
			}
			return document;
		}
	}
}
