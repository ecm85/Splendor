using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Splendor
{
	public class ImageCreator
	{
		private const float cardShortSideInInches = 2.5f;
		private const float cardLongSideInInches = 3.5f;
		//card: 2.5x3.5 = 240 * 336
		private const int shortSideDpi = 96;
		private const int longSideDpi = 96;
		private const int cardShortSideInPixels = (int)(shortSideDpi * cardShortSideInInches);
		private const int cardLongSideInPixels = (int)(longSideDpi * cardLongSideInInches);

		private readonly Color standardCardBackgroundColor = Color.BurlyWood;
		private const string questBackText = "Quest";
		private readonly FontFamily headerFontFamily = new FontFamily("Tempus Sans ITC");
		private static readonly FontFamily bodyFontFamily = new FontFamily("Calibri");
		private static readonly FontFamily cardBackFontFamily = new FontFamily("Cambria");

		private readonly StringFormat horizontalCenterAlignment = new StringFormat { Alignment = StringAlignment.Center};
		private readonly StringFormat fullCenterAlignment = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
		private readonly StringFormat verticalFarAlignment = new StringFormat {LineAlignment = StringAlignment.Far};
		private readonly StringFormat horizontalNearAlignment = new StringFormat {Alignment = StringAlignment.Near};
		private readonly StringFormat horizontalFarAlignment = new StringFormat {Alignment = StringAlignment.Far};
		private readonly SolidBrush blackBrush = new SolidBrush(Color.Black);

		private const int resourceKeyImageSize = 40;
		private const int arrowImageSize = 10;
		private const int questCostImageSize = 40;
		private const int wreathImageWidth = 55;
		private const int wreathImageHeight = 50;
		private const int questFrontHeaderFontSize = 14;
		private const int bodyFontSize = 12;
		private const int questImageY = 130;
		private const int questBackTextSize = 40;
		private const int borderPadding = 12;

		private static int ArrowPadding => arrowImageSize / 2;

		public Image CreateQuestBack()
		{
			var cardBackBitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(cardBackBitmap);

			PrintCardBorder(graphics, null, cardShortSideInPixels, cardLongSideInPixels, standardCardBackgroundColor);
			DrawIconPentagon(graphics);
			PrintCardBackString(graphics, questBackText, questBackTextSize);
			PrintGameTitle(graphics);
			return cardBackBitmap;
		}

		public Image CreateQuestFront(Quest quest)
		{
			var bitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, cardShortSideInPixels, cardLongSideInPixels, standardCardBackgroundColor);
			var headerFont = new Font(headerFontFamily, questFrontHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			var headerHeight = headerFont.Height;
			graphics.DrawString(
				quest.Name,
				headerFont,
				blackBrush,
				new RectangleF(borderPadding, borderPadding, cardShortSideInPixels - 2 * borderPadding, headerHeight),
				horizontalCenterAlignment);
			var questDescriptionFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);
			var questDescriptionHeight = questDescriptionFont.Height * 3;
			graphics.DrawString(quest.Description,
				questDescriptionFont,
				blackBrush,
				new RectangleF(
					borderPadding,
					headerHeight + borderPadding,
					cardShortSideInPixels - 2 * borderPadding,
					questDescriptionHeight),
				horizontalCenterAlignment);
			PrintCostsForQuest(graphics, quest, borderPadding, borderPadding + headerHeight + questDescriptionHeight);
			PrintPointsForQuest(graphics, quest, cardShortSideInPixels - (borderPadding + wreathImageWidth), borderPadding + headerHeight + questDescriptionHeight);
			PrintScaledJpg(graphics, quest.Image, borderPadding, questImageY, cardShortSideInPixels - 2 * borderPadding, cardLongSideInPixels - (questImageY + borderPadding));
			return bitmap;
		}

		private void PrintCostsForQuest(Graphics graphics, Quest quest, int initialX, int initialY)
		{
			for (var toolIndex = 0; toolIndex < quest.ToolRequirements.Count; toolIndex++)
			{
				PrintImageWithText(
					graphics,
					$"{quest.ToolRequirements[toolIndex]} BW",
					initialX + (questCostImageSize / 4) + toolIndex * (questCostImageSize + (questCostImageSize / 8) + questCostImageSize / 4),
					initialY,
					questCostImageSize,
					quest.ToolCountRequired.ToString(),
					(int)-(questCostImageSize / 2.6f),
					0);
			}
		}

		private void PrintPointsForQuest(Graphics graphics, Quest quest, int initialX, int initialY)
		{
			PrintPoints(graphics, quest.Points, initialX, initialY);
		}

		public Image CreateQuestAidFront()
		{
			var bitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(bitmap);

			PrintCardBorder(graphics, null, cardShortSideInPixels, cardLongSideInPixels, standardCardBackgroundColor);

			var questAidTitle = "Completing Quests";
			var titleRectangle = new RectangleF(
				borderPadding,
				borderPadding,
				cardShortSideInPixels - 2 * borderPadding,
				cardLongSideInPixels - 2 * borderPadding);
			var titleFont = new Font(headerFontFamily, questFrontHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			graphics.DrawString(questAidTitle, titleFont, blackBrush, titleRectangle, horizontalCenterAlignment);

			var textRectangle = new RectangleF(
				borderPadding,
				borderPadding + titleFont.Height,
				cardShortSideInPixels - 2 * borderPadding,
				cardLongSideInPixels - (2 * borderPadding + titleFont.Height));
			var textFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);

			var questAidString = "After completing your action each day, check if you have the tools depicted on each quest." +
				"\r\n\r\nIf you do, equip your villagers with those tools and they will complete the quest for you. Take the quest card and place in front of you." +
				"\r\n\r\nDon't worry, they'll return the tools in the same condition (more or less). ";
			graphics.DrawString(questAidString, textFont, blackBrush, textRectangle);

			var questSetupString = "Setup: Reveal 3 Quest cards and return the rest to the box. ";
			graphics.DrawString(questSetupString, textFont, blackBrush, textRectangle, verticalFarAlignment);

			return bitmap;
		}

		public Image CreatePlayerAidFront()
		{
			var bitmap = new Bitmap(cardLongSideInPixels, cardShortSideInPixels);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, cardLongSideInPixels, cardShortSideInPixels, standardCardBackgroundColor);
			var playerAidString = "Each day, you may take one of the following actions:" +
				"\r\n\u2022  Scavenge for Resources: gather 3 different resources" +
				"\r\n\u2022  Hunt for Resources: gather 2 of the same resource [as long as there is an abundance (4+)]" +
				"\r\n\u2022  Find Blueprint: Reserve a Tool from the display into your hand and take 1 gold [if available]" +
				"\r\n\u2022  Craft: Take a Tool from the display, place in front of you and return the depicted resources to the supply" +
				"\r\n\r\nAfter your action, check if you have the tools to complete any quests.";
			graphics.DrawString(
				playerAidString,
				new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel),
				blackBrush,
				new RectangleF(borderPadding, borderPadding, cardLongSideInPixels - 2 * borderPadding, cardShortSideInPixels - 2 * borderPadding));
			PrintLimitsReminder(graphics);
			return bitmap;
		}

		public Image CreatePlayerAidBack()
		{
			var bitmap = new Bitmap(cardLongSideInPixels, cardShortSideInPixels);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, cardLongSideInPixels, cardShortSideInPixels, standardCardBackgroundColor);
			PrintLimitsReminder(graphics);

			var columnWidth = resourceKeyImageSize * 2 + arrowImageSize + 2 * ArrowPadding;
			var columnPadding = 3 * (cardLongSideInPixels - 2*columnWidth)/7;
			var columnOffset = 2 * (cardLongSideInPixels - 2*columnWidth)/7;
			var firstColumnX = columnOffset;
			var secondColumnX = columnOffset + columnWidth + columnPadding;
			var rowWidth = resourceKeyImageSize;
			var rowPadding = (cardShortSideInPixels - 3*rowWidth)/5;
			var rowOffset = rowPadding;
			var firstRowY = rowOffset;
			var secondRowY = rowOffset + rowWidth + rowPadding;
			var thirdRowY = rowOffset + 2*(rowWidth + rowPadding);

			PrintImageMappingPng(graphics, "Axe BW", "Axe", "Wood", "Wood", firstColumnX, firstRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Sword BW", "Sword", "Dragonbone", "Dragonbone", secondColumnX, firstRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Staff BW", "Staff", "Magic", "Magic Shards", firstColumnX, secondRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Pick BW", "Pick", "Iron", "Iron Ore", secondColumnX, secondRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Chisel BW", "Chisel", "Stone", "Stone", firstColumnX, thirdRowY, resourceKeyImageSize);
			using (var goldImage = Image.FromFile("Images\\Gold.png"))
				PrintImageMapping(graphics, CreateToolCardBack(3), "Reserve", goldImage, "Gold (wild)", secondColumnX, thirdRowY, resourceKeyImageSize);

			return bitmap;
		}

		private void PrintLimitsReminder(Graphics graphics)
		{
			var handLimitString = "Hand limit - 3";
			var resourceLimitString = "Resource limit - 10";
			var font = new Font(bodyFontFamily, 9);
			graphics.DrawString(
				handLimitString,
				font,
				blackBrush,
				new RectangleF(borderPadding, cardShortSideInPixels - (15 + borderPadding), cardLongSideInPixels - (15 + borderPadding), 50),
				horizontalNearAlignment);
			graphics.DrawString(
				resourceLimitString,
				font,
				blackBrush,
				new RectangleF(borderPadding, cardShortSideInPixels - (15 + borderPadding), cardLongSideInPixels - (15 + borderPadding), 50),
				horizontalFarAlignment);
		}

		private void PrintImageMappingPng(Graphics graphics, string filename1, string label1, string filename2, string label2, int x, int y, int imageSize)
		{
			using (var image1 = Image.FromFile($"Images\\{filename1}.png"))
			using (var image2 = Image.FromFile($"Images\\{filename2}.png"))
			{
				PrintImageMapping(graphics, image1, label1, image2, label2, x, y, imageSize);
			}
		}

		private void PrintImageMapping(Graphics graphics, Image image1, string label1, Image image2, string label2, int x, int y, int imageSize)
		{
			var arrowPadding = ArrowPadding;
			var font = new Font(bodyFontFamily, 9);
			var brush = new SolidBrush(Color.Black);
			var label1Rectangle = new RectangleF(x - 30, y + imageSize, imageSize + 60, 20);
			var label2Rectangle = new RectangleF(x + imageSize + arrowPadding + arrowImageSize + arrowPadding - 30, y + imageSize, imageSize + 60, 20);

			PrintScaledImage(graphics, image1, x, y, imageSize, imageSize);
			graphics.DrawString(label1, font, brush, label1Rectangle, horizontalCenterAlignment);
			PrintScaledPng(graphics, "arrow", x + imageSize + arrowPadding, y + (imageSize / 2), arrowImageSize, arrowImageSize);
			PrintScaledImage(graphics, image2, x + imageSize + arrowPadding + arrowImageSize + arrowPadding, y, imageSize, imageSize);
			graphics.DrawString(label2, font, brush, label2Rectangle, horizontalCenterAlignment);
		}

		public Image CreateToolCardBack(int tier)
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
			PrintCardBackString(graphics, new string(Enumerable.Repeat('I', tier).ToArray()), 60);
		}

		private void PrintCardBackString(Graphics graphics, string text, int textSize)
		{
			graphics.DrawString(
				text,
				new Font(cardBackFontFamily, textSize, FontStyle.Bold),
				new SolidBrush(Color.Black),
				new RectangleF(8, 13, cardShortSideInPixels - 13, cardLongSideInPixels - 13),
				fullCenterAlignment);
		}

		private void PrintGameTitle(Graphics graphics)
		{
			var font = new Font(cardBackFontFamily, 35);
			var brush = new SolidBrush(Color.Black);
			graphics.DrawString(
				"Splendor",
				font,
				brush,
				new RectangleF(8, 13, cardShortSideInPixels - 8, 100),
				horizontalCenterAlignment);
			graphics.DrawString(
				"Forge",
				font,
				brush,
				new RectangleF(8, cardLongSideInPixels - 83, cardShortSideInPixels - 8, 100),
				horizontalCenterAlignment);
		}

		public Image CreateToolCardFront(NewCard newCard)
		{
			var cardBitmap = new Bitmap(cardShortSideInPixels, cardLongSideInPixels);
			var graphics = Graphics.FromImage(cardBitmap);
			PrintCardBorder(graphics, newCard.Color, cardShortSideInPixels, cardLongSideInPixels, standardCardBackgroundColor);
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
			var fontFamily = headerFontFamily;
			const int fontSize = 14;
			var font = new Font(fontFamily, fontSize);
			var brush = new SolidBrush(Color.Black);
			const int fontLineHeight = 25;
			var topRectangle = new RectangleF(0, borderPadding, cardWidth, fontLineHeight);
			var bottomRectangle = new RectangleF(0, fontLineHeight + borderPadding, cardWidth, fontLineHeight);
			var nameParts = newCard.Name.Split(' ');
			var firstNamePart = nameParts.Take(nameParts.Length - 2).ToList();
			var lastNamePart = nameParts.Skip(firstNamePart.Count).ToList();
			graphics.DrawString(string.Join(" ", firstNamePart), font, brush, topRectangle, fullCenterAlignment);
			graphics.DrawString(string.Join(" ", lastNamePart), font, brush, bottomRectangle, fullCenterAlignment);
		}

		private static void PrintToolImage(NewCard newCard, Graphics graphics)
		{
			PrintScaledPng(graphics, newCard.Tool, 65, 130, cardShortSideInPixels - 130, cardShortSideInPixels - 130);
		}

		private static void PrintResourceProduced(NewCard newCard, Graphics graphics)
		{
			PrintImageWithText(graphics, newCard.ResourceProduced, cardShortSideInPixels - 65, 65, 50, "+", -1, 20);
		}

		private void PrintToolIcon(NewCard newCard, Graphics graphics)
		{
			PrintScaledPng(graphics, $"{newCard.Tool} BW", borderPadding, borderPadding + 50, 50, 50);
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
			var textX = imageX + textImageXOffset;
			var textY = imageY + textImageYOffset;
			var path = new GraphicsPath();
			path.AddString(text, bodyFontFamily, 0, 30, new PointF(textX, textY), new StringFormat());
			graphics.FillPath(Brushes.White, path);
			graphics.DrawPath(new Pen(Color.Black, .5f), path);
		}

		private void PrintPointsForTool(Graphics graphics, NewCard newCard)
		{
			PrintPoints(graphics, newCard.Points, cardShortSideInPixels - (borderPadding + wreathImageWidth), cardLongSideInPixels - (borderPadding + wreathImageHeight));
		}

		private void PrintPoints(Graphics graphics, int points, int x, int y)
		{
			PrintScaledPng(graphics, "Wreath", x, y, wreathImageWidth, wreathImageHeight);
			var path = new GraphicsPath();
			path.AddString(points.ToString(), bodyFontFamily, 0, 30, new RectangleF(x, y, wreathImageWidth, wreathImageHeight), horizontalCenterAlignment);
			graphics.FillPath(Brushes.White, path);
			graphics.DrawPath(new Pen(Color.Black, .5f), path);
		}

		private static void PrintScaledPng(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			using (var srcImage = Image.FromFile($"Images\\{fileName}.png"))
			{
				PrintScaledImage(graphics, srcImage, x, y, width, height);
			}
		}

		private static void PrintScaledJpg(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			using (var srcImage = Image.FromFile($"Images\\{fileName}.jpg"))
			{
				PrintScaledImage(graphics, srcImage, x, y, width, height);
			}
		}

		private static void PrintScaledImage(Graphics graphics, Image image, int x, int y, int width, int height)
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphics.DrawImage(image, new Rectangle(x, y, width, height));
		}
	}
}
