using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Splendor
{
	public class ImageCreator
	{
		//page: 8.5x11 = 816 * 1056
		//card: 2.5x3.5 = 240 * 336
		private const int cardShortSideInPixels = 240;
		private const int cardLongSideInPixels = 336;
		private const int cardCostSize = 30;

		public Image CreateQuestFront(Quest quest)
		{
			var bitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, cardShortSideInPixels, cardLongSideInPixels, Color.BurlyWood);
			var fontFamily = new FontFamily("Tempus Sans ITC");
			graphics.DrawString(quest.Name, new Font(fontFamily, 11, FontStyle.Bold), new SolidBrush(Color.Black), new RectangleF(15, 15, cardShortSideInPixels - 30, 45), new StringFormat { Alignment = StringAlignment.Center});
			graphics.DrawString(quest.Description, new Font(fontFamily, 9), new SolidBrush(Color.Black), new RectangleF(15, 30, cardShortSideInPixels - 30, 60), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center});
			PrintScaledJpg(graphics, quest.Image, 15, 130, cardShortSideInPixels - 30, cardLongSideInPixels - (130 + 15));
			PrintCostsForQuest(graphics, quest);
			return bitmap;
		}

		private static void PrintCostsForQuest(Graphics graphics, Quest quest)
		{
			for (var toolIndex = 0; toolIndex < quest.ToolRequirements.Count; toolIndex++)
				PrintImageWithText(graphics, $"{quest.ToolRequirements[toolIndex]} BW", 15 + toolIndex * (40 + 5), 80, 40, quest.ToolCountRequired.ToString(), 5, 5);
		}

		private static void PrintPointsForQuest()
		{

		}

		public Image CreatePlayerAidFront()
		{
			var bitmap = new Bitmap(cardLongSideInPixels, cardShortSideInPixels);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, cardLongSideInPixels, cardShortSideInPixels, Color.BurlyWood);
			var playerAidString = "Each day, you may take one of the following actions:" +
				"\r\n\u2022  Gather Resources: gather 3 different resources" +
				"\r\n\u2022  Seek Resources: gather 2 of the same resource [as long as there is an abundance (4+)]" +
				"\r\n\u2022  Craft: Take a Tool from the display, place in front of you and return the depicted resources to the supply" +
				"\r\n\u2022  Find Blueprint: Take a Tool from the display into your hand and take one (1) gold [if available]" +
				"\r\n\r\nAfter your action, check if you have the tools to complete any quests.";
			graphics.DrawString(playerAidString, new Font(new FontFamily("Tempus Sans ITC"), 10), new SolidBrush(Color.Black), new RectangleF(15, 15, cardLongSideInPixels - 20, cardShortSideInPixels - 15), new StringFormat());
			PrintLimitsReminder(graphics);
			return bitmap;
		}

		public Image CreatePlayerAidBack()
		{
			var bitmap = new Bitmap(cardLongSideInPixels, cardShortSideInPixels);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, cardLongSideInPixels, cardShortSideInPixels, Color.BurlyWood);
			PrintLimitsReminder(graphics);

			var firstX = 45;
			var firstY = 15;
			var imageSize = 40;
			var columnPadding = 150;
			var rowPadding = 25;
			PrintImageMapping(graphics, "Axe BW", "Axe", "Wood", "Wood", firstX, firstY, imageSize);
			PrintImageMapping(graphics, "Sword BW", "Sword", "Dragonbone", "Dragonbone", firstX + columnPadding, firstY, imageSize);
			PrintImageMapping(graphics, "Staff BW", "Staff", "Magic", "Magic Shards",firstX, firstY + imageSize + rowPadding, imageSize);
			PrintImageMapping(graphics, "Pick BW", "Pick", "Iron", "Iron Ore", firstX + columnPadding, firstY + imageSize + rowPadding, imageSize);
			PrintImageMapping(graphics, "Chisel BW", "Chisel", "Stone", "Stone", firstX + (columnPadding / 2), firstY + (imageSize + rowPadding) * 2, imageSize);

			return bitmap;
		}

		private static void PrintLimitsReminder(Graphics graphics)
		{
			var handLimitString = "Hand limit: 3";
			var resourceLimitString = "Resource limit: 10";
			var font = new Font(new FontFamily("Tempus Sans ITC"), 9);
			var solidBrush = new SolidBrush(Color.Black);
			graphics.DrawString(
				handLimitString,
				font,
				solidBrush,
				new RectangleF(15, cardShortSideInPixels - 30, cardLongSideInPixels - 30, 50),
				new StringFormat
				{
					Alignment = StringAlignment.Near
				});
			graphics.DrawString(
				resourceLimitString,
				font,
				solidBrush,
				new RectangleF(15, cardShortSideInPixels - 30, cardLongSideInPixels - 30, 50),
				new StringFormat
				{
					Alignment = StringAlignment.Far
				});
		}

		private void PrintImageMapping(Graphics graphics, string filename1, string label1, string filename2, string label2, int x, int y, int imageSize)
		{
			var arrowSide = 10;
			var arrowPadding = 5;
			var labelPadding = 0;
			var font = new Font(new FontFamily("Tempus Sans ITC"), 9);
			var brush = new SolidBrush(Color.Black);
			var label1Rectangle = new RectangleF(x - 30, y + imageSize + labelPadding, imageSize + 60, 20);
			var label2Rectangle = new RectangleF(x + imageSize + arrowPadding + arrowSide + arrowPadding - 30, y + imageSize + labelPadding, imageSize + 60, 20);
			var stringFormat = new StringFormat
			{
				Alignment = StringAlignment.Center
			};

			PrintScaledPng(graphics, filename1, x, y, imageSize, imageSize);
			graphics.DrawString(label1, font, brush, label1Rectangle, stringFormat);
			PrintScaledPng(graphics, "arrow", x + imageSize + arrowPadding, y + (imageSize / 2), arrowSide, arrowSide);
			PrintScaledPng(graphics, filename2, x + imageSize + arrowPadding + arrowSide + arrowPadding, y, imageSize, imageSize);
			graphics.DrawString(label2, font, brush, label2Rectangle, stringFormat);
		}

		public Image CreateCardBackImage(int tier)
		{
			var cardBackBitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(cardBackBitmap);

			PrintCardBorder(graphics, null, cardShortSideInPixels, cardLongSideInPixels, GetTierBackColor(tier));
			DrawIconPentagon(graphics);
			PrintTier(graphics, tier);
			PrintGameTitle(graphics);
			return cardBackBitmap;
		}

		private Color GetTierBackColor(int tier)
		{
			switch (tier)
			{
				case 1:
					return Color.FromArgb(255, 150, 90, 56);
				case 2:
					return Color.FromArgb(255, 168, 168, 168);
				case 3:
					return Color.FromArgb(255, 201, 137, 16);
				default:
					throw new InvalidOperationException("Invalid tier!");
			}
		}

		private void DrawIconPentagon(Graphics graphics)
		{
			var radius = 85;

			var x = 119;
			var y = 170;
			var c1 = (int)(Math.Cos(2*Math.PI/5) * radius);
			var c2 = (int)(Math.Cos(Math.PI/5) * radius);
			var s1 = (int)(Math.Sin(2*Math.PI/5) * radius);
			var s2 = (int)(Math.Sin(4*Math.PI/5) * radius);


			var points = new[]
			{
				new Point(cardShortSideInPixels - x, cardLongSideInPixels - (y + radius)),
				new Point(cardShortSideInPixels - (x + s1), cardLongSideInPixels - (y + c1)),
				new Point(cardShortSideInPixels - (x + s2), cardLongSideInPixels - (y + -c2)),
				new Point(cardShortSideInPixels - (x + -s2), cardLongSideInPixels - (y + -c2)),
				new Point(cardShortSideInPixels - (x + -s1), cardLongSideInPixels - (y + c1))
			};

			var cardSide = 30;
			var halfCardSide = cardSide / 2;
			PrintScaledPng(graphics, "Axe BW", points[0].X - halfCardSide, points[0].Y - halfCardSide, cardSide, cardSide);
			PrintScaledPng(graphics, "Sword BW", points[1].X - halfCardSide, points[1].Y - halfCardSide, cardSide, cardSide);
			PrintScaledPng(graphics, "Staff BW", points[2].X - halfCardSide, points[2].Y - halfCardSide, cardSide, cardSide);
			PrintScaledPng(graphics, "Pick BW", points[3].X - halfCardSide, points[3].Y - halfCardSide, cardSide, cardSide);
			PrintScaledPng(graphics, "Chisel BW", points[4].X - halfCardSide, points[4].Y - halfCardSide, cardSide, cardSide);
		}

		private void PrintTier(Graphics graphics, int tier)
		{
			var tierString = new string(Enumerable.Repeat('I', tier).ToArray());
			graphics.DrawString(
				tierString,
				new Font(new FontFamily("Cambria"), 60),
				new SolidBrush(Color.Black),
				new RectangleF(8, 13, cardShortSideInPixels - 13, cardLongSideInPixels - 13),
				new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				});
		}

		private void PrintGameTitle(Graphics graphics)
		{
			var font = new Font(new FontFamily("Cambria"), 35);
			var brush = new SolidBrush(Color.Black);
			var stringFormat = new StringFormat()
			{
				Alignment = StringAlignment.Center
			};
			graphics.DrawString(
				"Splendor",
				font,
				brush,
				new RectangleF(8, 13, cardShortSideInPixels - 8, 100),
				stringFormat);
			graphics.DrawString(
				"Forge",
				font,
				brush,
				new RectangleF(8, cardLongSideInPixels - 83, cardShortSideInPixels - 8, 100),
				stringFormat);
		}

		public Image CreateCardImage(NewCard newCard)
		{
			var cardBitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(cardBitmap);
			PrintCardBorder(graphics, newCard.Color, cardShortSideInPixels, cardLongSideInPixels, Color.BurlyWood);
			PrintCardName(newCard, cardShortSideInPixels, graphics);
			PrintToolImage(newCard, graphics);
			PrintResourceProduced(newCard, graphics);
			PrintToolIcon(newCard, graphics);
			PrintCostsForTool(newCard, graphics);
			if (newCard.Points > 0)
				PrintPointsForTool(graphics, newCard);
			return cardBitmap;
		}

		private void PrintCardBorder(Graphics graphics, Color? middleBorderColor, int topSideInPixels, int leftSideInPixels, Color backgroundColor)
		{
			graphics.FillRoundedRectangle(new SolidBrush(Color.Black), 0, 0, topSideInPixels - 1, leftSideInPixels - 1, 10);
			if (middleBorderColor.HasValue)
				graphics.FillRoundedRectangle(new SolidBrush(middleBorderColor.Value), 5, 5, topSideInPixels - 11, leftSideInPixels - 11, 10);
			graphics.FillRoundedRectangle(new SolidBrush(backgroundColor), 10, 10, topSideInPixels-21, leftSideInPixels-21, 10);
		}

		private void PrintCardName(NewCard newCard, int cardWidth, Graphics graphics)
		{
			var fontFamily = new FontFamily("Tempus Sans ITC");
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
			PrintScaledPng(graphics, newCard.Tool, 65, 130, cardShortSideInPixels - 130, cardShortSideInPixels - 130);
		}

		private static void PrintResourceProduced(NewCard newCard, Graphics graphics)
		{
			PrintImageWithText(graphics, newCard.ResourceProduced, cardShortSideInPixels - 65, 65, 50, "+", -1, 20);
		}

		private static void PrintToolIcon(NewCard newCard, Graphics graphics)
		{
			PrintScaledPng(graphics, $"{newCard.Tool} BW", 15, 65, 50, 50);
		}

		private static void PrintCostsForTool(NewCard newCard, Graphics graphics)
		{
			var costList = newCard.Costs.ToList();
			for (var costIndex = 0; costIndex < costList.Count; costIndex++)
				PrintImageWithText(graphics, costList[costIndex].Key, 12, cardLongSideInPixels - (costIndex*50) - 70, 50, costList[costIndex].Value.ToString(), -1, 20);
		}

		private static void PrintImageWithText(Graphics graphics, string fileName, int imageX, int imageY, int imageSide, string text, int textImageXOffset, int textImageYOffset)
		{
			PrintScaledPng(graphics, fileName, imageX, imageY, imageSide, imageSide);
			var path = new GraphicsPath();
			path.AddString(text, new FontFamily("Arial"), 0, cardCostSize,
				new PointF(imageX + textImageXOffset, imageY + textImageYOffset), new StringFormat());
			graphics.FillPath(Brushes.White, path);
			graphics.DrawPath(new Pen(Color.Black, .5f), path);
		}

		private void PrintPointsForTool(Graphics graphics, NewCard newCard)
		{
			var fontFamily = new FontFamily("Tempus Sans ITC");
			var font = new Font(fontFamily, 25);
			var brush = new SolidBrush(Color.Black);
			PrintScaledPng(graphics, "Wreath", cardShortSideInPixels - 68, cardLongSideInPixels - 65, 65, 50);
			graphics.DrawString(newCard.Points.ToString(), font, brush, cardShortSideInPixels - 50, cardLongSideInPixels - 73);
		}

		private static void PrintScaledPng(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			PrintScaledImage(graphics, $"{fileName}.png", x, y, width, height);
		}

		private static void PrintScaledJpg(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			PrintScaledImage(graphics, $"{fileName}.jpg", x, y, width, height);
		}

		private static void PrintScaledImage(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			using (var srcImage = Image.FromFile($"Images\\{fileName}"))
			{
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.DrawImage(srcImage, new Rectangle(x, y, width, height));
			}
		}
	}
}
