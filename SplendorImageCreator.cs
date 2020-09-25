using System;
using System.Drawing;
using System.Linq;

namespace Splendor
{
	public class SplendorImageCreator
	{
		private readonly Color standardCardBackgroundColor = Color.BurlyWood;

		private const int limitsFontSize = (int) (10 * GraphicsUtilities.dpiFactor);
		private const int bodyFontSize = (int) (11 * GraphicsUtilities.dpiFactor);
		private const int questHeaderFontSize = (int) (13 * GraphicsUtilities.dpiFactor);
		private const int toolHeaderFontSize = (int) (20 * GraphicsUtilities.dpiFactor);
		private const int imageLabelFontSize = (int) (20 * GraphicsUtilities.dpiFactor);
		private const int gameTitleFontSize = (int) (38 * GraphicsUtilities.dpiFactor);
		private const int questBackFontSize = (int) (45 * GraphicsUtilities.dpiFactor);
		private const int tierTextFontSize = (int) (80 * GraphicsUtilities.dpiFactor);

		private const int resourceKeyImageSize = (int) (35 * GraphicsUtilities.dpiFactor);
		private const int arrowImageSize = (int) (10 * GraphicsUtilities.dpiFactor);
		private const int questCostImageSize = (int) (35 * GraphicsUtilities.dpiFactor);
		private const int pentagonImageSize = (int) (25 * GraphicsUtilities.dpiFactor);
		private const int wreathImageWidth = (int) (40 * GraphicsUtilities.dpiFactor);
		private const int cardFrontSmallImageSize = (int) (35 * GraphicsUtilities.dpiFactor);
		private const int questImageYBottomPadding = (int) (5 * GraphicsUtilities.dpiFactor);

		private static int ArrowPadding => arrowImageSize / 2;

		private readonly FontFamily headerFontFamily = new FontFamily("Tempus Sans ITC");
		private static readonly FontFamily bodyFontFamily = new FontFamily("Calibri");
		private static readonly FontFamily cardBackFontFamily = new FontFamily("Cambria");

		public CardImage CreateQuestBack()
		{
			var cardImage = new CardImage("Quest Back", ImageOrientation.Portrait);
			cardImage.PrintCardBorderAndBackground(null, Color.DodgerBlue);
			DrawIconPentagon(cardImage);
			PrintCardBackString(cardImage, "Quest", questBackFontSize);
			PrintGameTitle(cardImage);
			return cardImage;
		}

		public CardImage CreateQuestFront(Quest quest)
		{
			var cardImage = new CardImage($"Quest - {quest.Name}", ImageOrientation.Portrait);
			cardImage.PrintCardBorderAndBackground(null, standardCardBackgroundColor);
			var graphics = cardImage.Graphics;
			var headerFont = new Font(headerFontFamily, questHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			var headerHeight = headerFont.Height;
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;
			GraphicsUtilities.DrawString(
				graphics,
				quest.Name,
				headerFont,
				GraphicsUtilities.BlackBrush,
				new RectangleF(usableRectangleWithPadding.X, usableRectangleWithPadding.Y, usableRectangleWithPadding.Width, headerHeight),
				GraphicsUtilities.HorizontalCenterAlignment);
			var questDescriptionFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);
			var questDescriptionHeight = questDescriptionFont.Height * 3;
			GraphicsUtilities.DrawString(
				graphics,
				quest.Description,
				questDescriptionFont,
				GraphicsUtilities.BlackBrush,
				new RectangleF(usableRectangleWithPadding.X, usableRectangleWithPadding.Y + headerHeight, usableRectangleWithPadding.Width, questDescriptionHeight),
				GraphicsUtilities.HorizontalCenterAlignment);
			PrintCostsForQuest(graphics, quest, usableRectangleWithPadding.Left, usableRectangleWithPadding.Top + headerHeight + questDescriptionHeight);
			PrintPoints(graphics, quest.Points, usableRectangleWithPadding.Right - wreathImageWidth, usableRectangleWithPadding.Top + headerHeight + questDescriptionHeight);
			var questImageY = usableRectangleWithPadding.Top + headerHeight + questDescriptionHeight + cardFrontSmallImageSize;
			GraphicsUtilities.PrintScaledJpg(graphics, quest.Image, usableRectangleWithPadding.X, questImageY, usableRectangleWithPadding.Width, usableRectangleWithPadding.Height - (questImageY + questImageYBottomPadding));
			return cardImage;
		}

		private void PrintCostsForQuest(Graphics graphics, Quest quest, int initialX, int initialY)
		{
			var font = new Font(bodyFontFamily, imageLabelFontSize);
			for (var toolIndex = 0; toolIndex < quest.ToolRequirements.Count; toolIndex++)
			{
				GraphicsUtilities.PrintImageWithText(
					graphics,
					$"{quest.ToolRequirements[toolIndex]} BW",
					initialX + (questCostImageSize / 4) + toolIndex * (questCostImageSize + (questCostImageSize / 8) + questCostImageSize / 4),
					initialY,
					questCostImageSize,
					questCostImageSize,
					quest.ToolCountRequired.ToString(),
					(int)-(questCostImageSize / 2.6f),
					0,
					font);
			}
		}

		private void PrintPoints(Graphics graphics, int points, int x, int y)
		{
			var font = new Font(bodyFontFamily, imageLabelFontSize);
			GraphicsUtilities.PrintImageWithTextCentered(graphics, "Wreath", x, y, wreathImageWidth, cardFrontSmallImageSize, points.ToString(), font);
		}

		public CardImage CreateQuestAidFront()
		{
			var cardImage = new CardImage("Quest Aid", ImageOrientation.Portrait);
			var graphics = cardImage.Graphics;
			cardImage.PrintCardBorderAndBackground(null, standardCardBackgroundColor);
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;

			var questAidTitle = "Completing Quests";
			var titleFont = new Font(headerFontFamily, questHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			GraphicsUtilities.DrawString(graphics, questAidTitle, titleFont, GraphicsUtilities.BlackBrush, usableRectangleWithPadding);

			var textRectangle = new RectangleF(
				usableRectangleWithPadding.X,
				usableRectangleWithPadding.Y + titleFont.Height,
				usableRectangleWithPadding.Width,
				usableRectangleWithPadding.Height - titleFont.Height);
			var textFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);

			var questAidString = "After completing your action each day, check if you have the tools depicted on each quest." +
				"\r\n\r\nIf you do, equip your villagers with those tools and they will complete the quest for you. Take the quest card and place in front of you." +
				"\r\n\r\nDon't worry, they'll return the tools in the same condition (more or less). " +
				"\r\n\r\nYou only have enough villagers to complete one quest each day. If you gain the tool to complete two quests on the same day, one must be completed on the next day (or be stolen by someone else).";
			GraphicsUtilities.DrawString(graphics, questAidString, textFont, GraphicsUtilities.BlackBrush, textRectangle);

			return cardImage;
		}

		public CardImage CreateSetupAidFront()
		{
			var cardImage = new CardImage("Setup Aid", ImageOrientation.Portrait);
			var graphics = cardImage.Graphics;

			cardImage.PrintCardBorderAndBackground(null, standardCardBackgroundColor);

			var setupAidTitle = "Game Setup";
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;
			var titleFont = new Font(headerFontFamily, questHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			GraphicsUtilities.DrawString(graphics, setupAidTitle, titleFont, GraphicsUtilities.BlackBrush, usableRectangleWithPadding);

			var textRectangle = new RectangleF(
				usableRectangleWithPadding.X,
				usableRectangleWithPadding.Y + titleFont.Height,
				usableRectangleWithPadding.Width,
				usableRectangleWithPadding.Height - titleFont.Height);
			var textFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);

			var setupAidString =
				"\r\n\r\nResources:\r\nGold: Always 5\r\n4 Players: 7 of each other resource\r\n3 Players: 5 of each other resource\r\n2 Players: 4 of each other resource" +
				"\r\n\r\n\r\nQuests:\r\n 1 + Player Count" +
				"\r\n\r\n\r\nTools:\r\n4 per Tier";
			GraphicsUtilities.DrawString(graphics, setupAidString, textFont, GraphicsUtilities.BlackBrush, textRectangle);

			return cardImage;
		}

		public CardImage CreatePlayerAidFront()
		{
			var cardImage = new CardImage("Player Aid Front", ImageOrientation.Landscape);
			var graphics = cardImage.Graphics;

			cardImage.PrintCardBorderAndBackground(null, standardCardBackgroundColor);
			var playerAidString = "Each day, you may take one of the following actions:" +
				"\r\n\u2022  Scavenge for Resources: gather 3 different resources" +
				"\r\n\u2022  Hunt for Resources: gather 2 of the same resource [as long as there is an abundance (4+)]" +
				"\r\n\u2022  Find Blueprint: Reserve a Tool from the display into your hand and take 1 gold [if available]" +
				"\r\n\u2022  Craft: Take a Tool from the display, place in front of you and return the depicted resources to the supply" +
				"\r\n\r\nAfter your action, check if you have the tools to complete any quests.";
			GraphicsUtilities.DrawString(
				graphics,
				playerAidString,
				new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel),
				GraphicsUtilities.BlackBrush,
				cardImage.UsableRectangWithPadding);
			PrintLimitsReminder(cardImage);
			return cardImage;
		}

		public CardImage CreatePlayerAidBack()
		{
			var cardImage = new CardImage("Player Aid Back", ImageOrientation.Landscape);
			var graphics = cardImage.Graphics;

			cardImage.PrintCardBorderAndBackground(null, standardCardBackgroundColor);
			PrintLimitsReminder(cardImage);
			var usableRectangWithPadding = cardImage.UsableRectangWithPadding;

			var columnWidth = resourceKeyImageSize * 2 + arrowImageSize + 2 * ArrowPadding;
			var columnPadding = (usableRectangWithPadding.Width - 2 * columnWidth) / 3;
			var columnOffset = (usableRectangWithPadding.Width - 2 * columnWidth) / 3;
			var firstColumnX = usableRectangWithPadding.X + columnOffset;
			var secondColumnX = usableRectangWithPadding.X + columnOffset + columnWidth + columnPadding;
			var rowWidth = resourceKeyImageSize;
			var rowPadding = (usableRectangWithPadding.Height - 3 * rowWidth) / 5;
			var rowOffset = rowPadding;
			var firstRowY = usableRectangWithPadding.Y + rowOffset;
			var secondRowY = usableRectangWithPadding.Y + rowOffset + rowWidth + rowPadding;
			var thirdRowY = usableRectangWithPadding.Y + rowOffset + 2 * (rowWidth + rowPadding);

			PrintImageMappingPng(graphics, "Axe BW", "Axe", "Wood", "Wood", firstColumnX, firstRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Sword BW", "Sword", "Dragonbone", "Dragonbone", secondColumnX, firstRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Staff BW", "Staff", "Magic", "Magic Shards", firstColumnX, secondRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Pick BW", "Pick", "Iron", "Iron Ore", secondColumnX, secondRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Chisel BW", "Chisel", "Stone", "Stone", firstColumnX, thirdRowY, resourceKeyImageSize);
			using (var goldImage = Image.FromFile("Images\\Gold.png"))
				PrintImageMapping(graphics, CreateToolCardBack(3).Bitmap, "Reserve", goldImage, "Gold (wild)", secondColumnX, thirdRowY, resourceKeyImageSize);

			return cardImage;
		}

		private void PrintLimitsReminder(CardImage cardImage)
		{
			var graphics = cardImage.Graphics;
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;
			var handLimitString = "Hand limit - 3";
			var resourceLimitString = "Resource limit - 10";
			var limitsReminderFont = new Font(bodyFontFamily, limitsFontSize, GraphicsUnit.Pixel);
			var textRectangle = new RectangleF(
				usableRectangleWithPadding.X,
				usableRectangleWithPadding.Bottom - limitsReminderFont.Height,
				usableRectangleWithPadding.Width,
				limitsReminderFont.Height);
			GraphicsUtilities.DrawString(graphics, handLimitString, limitsReminderFont, GraphicsUtilities.BlackBrush, textRectangle, GraphicsUtilities.HorizontalNearAlignment);
			GraphicsUtilities.DrawString(graphics, resourceLimitString, limitsReminderFont, GraphicsUtilities.BlackBrush, textRectangle, GraphicsUtilities.HorizontalFarAlignment);
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
			var imageMappingFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);
			var mappingTextWidth = imageSize + 2 * (arrowImageSize + 2 * ArrowPadding);
			var label1Rectangle = new RectangleF(x - (arrowImageSize + 2 * ArrowPadding), y + imageSize, mappingTextWidth, imageMappingFont.Height);
			var label2Rectangle = new RectangleF(x + imageSize + 2 * ArrowPadding + arrowImageSize - (arrowImageSize + 2 * ArrowPadding), y + imageSize, mappingTextWidth, imageMappingFont.Height);

			GraphicsUtilities.PrintScaledImage(graphics, image1, x, y, imageSize, imageSize);
			GraphicsUtilities.DrawString(graphics, label1, imageMappingFont, GraphicsUtilities.BlackBrush, label1Rectangle, GraphicsUtilities.HorizontalCenterAlignment);
			GraphicsUtilities.PrintScaledPng(graphics, "arrow", x + imageSize + ArrowPadding, y + (imageSize / 2), arrowImageSize, arrowImageSize);
			GraphicsUtilities.PrintScaledImage(graphics, image2, x + imageSize + ArrowPadding + arrowImageSize + ArrowPadding, y, imageSize, imageSize);
			GraphicsUtilities.DrawString(graphics, label2, imageMappingFont, GraphicsUtilities.BlackBrush, label2Rectangle, GraphicsUtilities.HorizontalCenterAlignment);
		}

		public CardImage CreateToolCardBack(int tier)
		{
			var cardImage = new CardImage($"Tier {tier} Back", ImageOrientation.Portrait);
			cardImage.PrintCardBorderAndBackground(null, GetTierBackColor(tier));
			DrawIconPentagon(cardImage);
			PrintCardBackString(cardImage, new string(Enumerable.Repeat('I', tier).ToArray()), tierTextFontSize);
			PrintGameTitle(cardImage);
			return cardImage;
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

		private void DrawIconPentagon(CardImage cardImage)
		{
			var graphics = cardImage.Graphics;
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;
			var radius = (usableRectangleWithPadding.Width) * (7.0 / 10) / 1.902;

			var x = usableRectangleWithPadding.Width / 2;
			var y = usableRectangleWithPadding.Height / 2;
			var c1 = (int)(Math.Cos(2 * Math.PI / 5) * radius);
			var c2 = (int)(Math.Cos(Math.PI / 5) * radius);
			var s1 = (int)(Math.Sin(2 * Math.PI / 5) * radius);
			var s2 = (int)(Math.Sin(4 * Math.PI / 5) * radius);

			var points = new[]
			{
				new Point(usableRectangleWithPadding.Right - x, (int)(usableRectangleWithPadding.Bottom - (y + radius))),
				new Point(usableRectangleWithPadding.Right - (x + s1), usableRectangleWithPadding.Bottom - (y + c1)),
				new Point(usableRectangleWithPadding.Right - (x + s2), usableRectangleWithPadding.Bottom - (y + -c2)),
				new Point(usableRectangleWithPadding.Right - (x + -s2), usableRectangleWithPadding.Bottom - (y + -c2)),
				new Point(usableRectangleWithPadding.Right - (x + -s1), usableRectangleWithPadding.Bottom - (y + c1))
			};

			var halfPentagonImageSize = pentagonImageSize / 2;
			GraphicsUtilities.PrintScaledPng(graphics, "Axe BW", points[0].X - halfPentagonImageSize, points[0].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			GraphicsUtilities.PrintScaledPng(graphics, "Sword BW", points[1].X - halfPentagonImageSize, points[1].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			GraphicsUtilities.PrintScaledPng(graphics, "Staff BW", points[2].X - halfPentagonImageSize, points[2].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			GraphicsUtilities.PrintScaledPng(graphics, "Pick BW", points[3].X - halfPentagonImageSize, points[3].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			GraphicsUtilities.PrintScaledPng(graphics, "Chisel BW", points[4].X - halfPentagonImageSize, points[4].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
		}

		private void PrintGameTitle(CardImage cardImage)
		{
			var graphics = cardImage.Graphics;
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;
			var titleFont = new Font(cardBackFontFamily, gameTitleFontSize, GraphicsUnit.Pixel);
			GraphicsUtilities.DrawString(
				graphics,
				"Splendor",
				titleFont,
				GraphicsUtilities.BlackBrush,
				new RectangleF(usableRectangleWithPadding.X, usableRectangleWithPadding.Y, usableRectangleWithPadding.Width, titleFont.Height),
				GraphicsUtilities.HorizontalCenterAlignment);
			GraphicsUtilities.DrawString(
				graphics,
				"Forge",
				titleFont,
				GraphicsUtilities.BlackBrush,
				new RectangleF(usableRectangleWithPadding.X, usableRectangleWithPadding.Bottom - titleFont.Height, usableRectangleWithPadding.Width, titleFont.Height),
				GraphicsUtilities.HorizontalCenterAlignment);
		}

		public CardImage CreateToolCardFront(NewToolCard newCard, int index)
		{
			var cardImage = new CardImage($"Tier {newCard.Tier} Front - {newCard.Name} {index}", ImageOrientation.Portrait);
			cardImage.PrintCardBorderAndBackground(newCard.Color, standardCardBackgroundColor);
			var usableRectangWithPadding = cardImage.UsableRectangWithPadding;
			var graphics = cardImage.Graphics;
			if (newCard.Points > 0)
				PrintPointsForTool(newCard, cardImage);
			PrintToolIcon(newCard, graphics, usableRectangWithPadding.X + (usableRectangWithPadding.Width / 2) - (cardFrontSmallImageSize / 2), usableRectangWithPadding.Y);
			PrintResourceProduced(newCard, graphics, usableRectangWithPadding.Right - cardFrontSmallImageSize, usableRectangWithPadding.Y);
			PrintCardName(newCard, cardImage);
			PrintToolImage(newCard, cardImage);
			PrintCostsForTool(newCard, cardImage);
			return cardImage;
		}

		private void PrintCardName(NewToolCard newCard, CardImage cardImage)
		{
			var graphics = cardImage.Graphics;
			var usableRectangWithPadding = cardImage.UsableRectangWithPadding;

			var cardNameFont = new Font(headerFontFamily, toolHeaderFontSize, GraphicsUnit.Pixel);
			var topRectangle = new RectangleF(usableRectangWithPadding.X, usableRectangWithPadding.Y + cardFrontSmallImageSize, usableRectangWithPadding.Width, cardNameFont.Height);
			var bottomRectangle = new RectangleF(usableRectangWithPadding.X, usableRectangWithPadding.Y + cardFrontSmallImageSize + cardNameFont.Height, usableRectangWithPadding.Width, cardNameFont.Height);
			var nameParts = newCard.Name.Split(' ');
			var firstNamePart = nameParts.Take(nameParts.Length - 2).ToList();
			var lastNamePart = nameParts.Skip(firstNamePart.Count).ToList();
			GraphicsUtilities.DrawString(graphics, string.Join(" ", firstNamePart), cardNameFont, GraphicsUtilities.BlackBrush, topRectangle, GraphicsUtilities.FullCenterAlignment);
			GraphicsUtilities.DrawString(graphics, string.Join(" ", lastNamePart), cardNameFont, GraphicsUtilities.BlackBrush, bottomRectangle, GraphicsUtilities.FullCenterAlignment);
		}

		private void PrintToolImage(NewToolCard newCard, CardImage cardImage)
		{
			var graphics = cardImage.Graphics;
			var usableRectangWithPadding = cardImage.UsableRectangWithPadding;
			var cardFrontLargeImageSize = usableRectangWithPadding.Width - (2 * cardFrontSmallImageSize);
			GraphicsUtilities.PrintScaledPng(
				graphics,
				newCard.Tool,
				usableRectangWithPadding.X + usableRectangWithPadding.Width / 2 - cardFrontLargeImageSize / 2,
				usableRectangWithPadding.Y + usableRectangWithPadding.Height / 2 - (cardFrontLargeImageSize / 2),
				cardFrontLargeImageSize,
				cardFrontLargeImageSize);
		}

		private void PrintResourceProduced(NewToolCard newCard, Graphics graphics, int x, int y)
		{
			var font = new Font(bodyFontFamily, imageLabelFontSize);
			GraphicsUtilities.PrintImageWithText(
				graphics,
				newCard.ResourceProduced,
				x,
				y,
				cardFrontSmallImageSize,
				cardFrontSmallImageSize,
				"+",
				0,
				(int)(cardFrontSmallImageSize * (2.0/5)),
				font);
		}

		private void PrintToolIcon(NewToolCard newCard, Graphics graphics, int x, int y)
		{
			GraphicsUtilities.PrintScaledPng(graphics, $"{newCard.Tool} BW", x, y, cardFrontSmallImageSize, cardFrontSmallImageSize);
		}

		private void PrintCostsForTool(NewToolCard newCard, CardImage cardImage)
		{
			var font = new Font(bodyFontFamily, imageLabelFontSize);
			var graphics = cardImage.Graphics;
			var usableRectangWithPadding = cardImage.UsableRectangWithPadding;
			var costList = newCard.Costs.ToList();
			for (var costIndex = 0; costIndex < costList.Count; costIndex++)
				GraphicsUtilities.PrintImageWithText(
					graphics,
					costList[costIndex].Key,
					usableRectangWithPadding.X,
					usableRectangWithPadding.Bottom - ((costIndex + 1) * cardFrontSmallImageSize),
					cardFrontSmallImageSize,
					cardFrontSmallImageSize,
					costList[costIndex].Value.ToString(),
					0,
					(int)(cardFrontSmallImageSize * (2.0 / 5)),
					font);
		}

		private void PrintPointsForTool(NewToolCard newCard, CardImage cardImage)
		{
			var graphics = cardImage.Graphics;
			var usableRectangWithPadding = cardImage.UsableRectangWithPadding;
			PrintPoints(
				graphics,
				newCard.Points,
				usableRectangWithPadding.X,
				usableRectangWithPadding.Y);
		}

		public static void PrintCardBackString(CardImage cardImage, string text, int textSize)
		{
			var graphics = cardImage.Graphics;
			var usableRectangleWithPadding = cardImage.UsableRectangWithPadding;
			GraphicsUtilities.DrawString(
				graphics,
				text,
				new Font(cardBackFontFamily, textSize, FontStyle.Bold, GraphicsUnit.Pixel),
				new SolidBrush(Color.Black),
				usableRectangleWithPadding,
				GraphicsUtilities.FullCenterAlignment);
		}
	}
}
