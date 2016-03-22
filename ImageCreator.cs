using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
	public class ImageCreator
	{
		//page: 8.5x11 = 816 * 1056
		//card: 2.5x3.5 = 240 * 336
		private const int cardWidth = 240;
		private const int cardHeight = 336;

		public Image CreateCardImage(NewCard newCard)
		{

			var cardBitmap = new Bitmap(cardWidth, cardHeight);
			var graphics = Graphics.FromImage(cardBitmap);
			PrintCardBorder(graphics);
			PrintCardName(newCard, cardWidth, graphics);
			PrintToolImage(newCard, graphics);
			PrintScaledImage(graphics, newCard.ResourceProduced, cardWidth - 52, 50, 50, 50);
			PrintScaledImage(graphics, $"{newCard.Tool} BW", 2, 50, 50, 50);
			var costList = newCard.Costs.ToList();
			for (var costIndex = 0; costIndex < costList.Count; costIndex++)
			{
				var cost = costList[costIndex];
				PrintBigString(graphics, cost.Value.ToString(), -10, cardHeight - (costIndex * 50) - 100);
				PrintScaledImage(graphics, cost.Key, 50, cardHeight - (costIndex * 50) - 60, 50, 50);
			}
			PrintPlusSign(graphics);
			return cardBitmap;
		}

		private void PrintPlusSign(Graphics graphics)
		{
			PrintBigString(graphics, "+", cardWidth - 112, 15);
		}

		private static void PrintBigString(Graphics graphics, string text, int x, int y)
		{
			var fontFamily = new FontFamily("Papyrus");
			const int fontSize = 60;
			var font = new Font(fontFamily, fontSize);
			var brush = new SolidBrush(Color.Black);
			graphics.DrawString(text, font, brush, x, y);
		}

		private static void PrintToolImage(NewCard newCard, Graphics graphics)
		{
			PrintScaledImage(graphics, newCard.Tool, 0, 100, cardWidth, cardWidth);
		}

		private static void PrintScaledImage(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			using (var srcImage = Image.FromFile($"Images\\{fileName}.png"))
			{
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.DrawImage(srcImage, new Rectangle(x, y, width, height));
			}
		}

		private void PrintCardBorder(Graphics graphics)
		{
			var brush = new SolidBrush(Color.BurlyWood);
			const int cardCornerRadius = 5;
			graphics.FillRoundedRectangle(brush, 0, 0, cardWidth - 1, cardHeight - 1, cardCornerRadius);
		}

		private void PrintCardName(NewCard newCard, int cardWidth, Graphics graphics)
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
