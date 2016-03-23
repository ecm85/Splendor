using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Splendor
{
	public class ImageCreator
	{
		//page: 8.5x11 = 816 * 1056
		//card: 2.5x3.5 = 240 * 336
		private const int cardWidth = 240;
		private const int cardHeight = 336;
		private const int cardCostSize = 30;

		public Image CreateCardImage(NewCard newCard)
		{

			var cardBitmap = new Bitmap(cardWidth, cardHeight);
			var graphics = Graphics.FromImage(cardBitmap);
			PrintCardBorder(graphics, newCard);
			PrintCardName(newCard, cardWidth, graphics);
			PrintToolImage(newCard, graphics);
			PrintResourceProduced(newCard, graphics);
			PrintToolIcon(newCard, graphics);
			PrintCosts(newCard, graphics);
			if (newCard.Points > 0)
				PrintPoints(graphics, newCard);
			return cardBitmap;
		}

		private void PrintCardBorder(Graphics graphics, NewCard newCard)
		{
			graphics.FillRoundedRectangle(new SolidBrush(Color.Black), 0, 0, cardWidth - 1, cardHeight - 1, 10);
			graphics.FillRoundedRectangle(new SolidBrush(newCard.Color), 5, 5, cardWidth - 11, cardHeight - 11, 10);
			graphics.FillRoundedRectangle(new SolidBrush(Color.BurlyWood), 10, 10, cardWidth-21, cardHeight-21, 10);
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
			const int fontTopPadding = 15;
			const int fontLineHeight = 25;
			var topRectangle = new RectangleF(0, fontTopPadding, cardWidth, fontLineHeight);
			var bottomRectangle = new RectangleF(0, fontLineHeight + fontTopPadding, cardWidth, fontLineHeight);
			var nameParts = newCard.Name.Split(' ');
			var firstNamePart = nameParts.Take(nameParts.Length - 2).ToList();
			var lastNamePart = nameParts.Skip(firstNamePart.Count).ToList();
			graphics.DrawString(string.Join(" ", firstNamePart), font, brush, topRectangle, stringFormat);
			graphics.DrawString(string.Join(" ", lastNamePart), font, brush, bottomRectangle, stringFormat);
		}

		private static void PrintToolImage(NewCard newCard, Graphics graphics)
		{
			PrintScaledImage(graphics, newCard.Tool, 15, 115, cardWidth - 26, cardWidth - 26);
		}

		private static void PrintResourceProduced(NewCard newCard, Graphics graphics)
		{
			PrintScaledImage(graphics, newCard.ResourceProduced, cardWidth - 65, 65, 50, 50);
		}

		private static void PrintToolIcon(NewCard newCard, Graphics graphics)
		{
			PrintScaledImage(graphics, $"{newCard.Tool} BW", 15, 65, 50, 50);
		}

		private static void PrintCosts(NewCard newCard, Graphics graphics)
		{
			var costList = newCard.Costs.ToList();
			for (var costIndex = 0; costIndex < costList.Count; costIndex++)
			{
				var cost = costList[costIndex];
				PrintScaledImage(graphics, cost.Key, 12, cardHeight - (costIndex*50) - 70, 50, 50);
				var path = new GraphicsPath();
				path.AddString(cost.Value.ToString(), new FontFamily("Arial"), 0, cardCostSize,
					new PointF(11, cardHeight - (costIndex*50) - 50), new StringFormat());
				graphics.FillPath(Brushes.White, path);
				graphics.DrawPath(new Pen(Color.Black, .5f), path);
			}
		}

		private void PrintPoints(Graphics graphics, NewCard newCard)
		{
			var fontFamily = new FontFamily("Papyrus");
			var font = new Font(fontFamily, 25);
			var brush = new SolidBrush(Color.Black);
			PrintScaledImage(graphics, "Wreath", cardWidth - 68, cardHeight - 65, 65, 50);
			graphics.DrawString(newCard.Points.ToString(), font, brush, cardWidth - 50, cardHeight - 73);
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
	}
}
