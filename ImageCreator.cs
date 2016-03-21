using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
	public static class ImageCreator
	{
		//page: 8.5x11 = 816 * 1056
		//card: 2.5x3.5 = 240 * 336
		private const int cardWidth = 240;
		private const int cardHeight = 336;

		public static Image CreateCardImage(NewCard newCard)
		{

			var cardBitmap = new Bitmap(cardWidth, cardHeight);
			var graphics = Graphics.FromImage(cardBitmap);
			PrintCardBorder(graphics);
			PrintCardName(newCard, cardWidth, graphics);
			return cardBitmap;
		}

		private static void PrintCardBorder(Graphics graphics)
		{
			var borderPen = new Pen(Color.Blue);
			const int cardCornerRadius = 5;
			graphics.DrawRoundedRectangle(borderPen, 0, 0, cardWidth - 1, cardHeight - 1, cardCornerRadius);
		}

		private static void PrintCardName(NewCard newCard, int cardWidth, Graphics graphics)
		{
			var fontFamily = new FontFamily("Papyrus");
			const int fontSize = 14;
			var font = new Font(fontFamily, fontSize);
			var brush = new SolidBrush(Color.Black);
			var stringFormat = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			const int fontTopPadding = 5;
			const int fontLineHeight = 25;
			var topRectangle = new RectangleF(0, fontTopPadding, cardWidth, fontLineHeight);
			var bottomRectangle = new RectangleF(0, fontLineHeight + fontTopPadding, cardWidth, fontLineHeight);
			var nameParts = newCard.Name.Split(' ');
			var firstNamePart = nameParts.Take(nameParts.Length - 2).ToList();
			var lastNamePart = nameParts.Skip(firstNamePart.Count).ToList();
			graphics.DrawString(string.Join(" ", firstNamePart), font, brush, topRectangle, stringFormat);
			graphics.DrawString(string.Join(" ", lastNamePart), font, brush, bottomRectangle, stringFormat);
		}
	}
}
