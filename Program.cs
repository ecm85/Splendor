using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Splendor
{
	class Program
	{
		//Oh noes! The following changes were made and then lost (there may be others!)
		//1. Changed images to fit copyright issues
		//2. Changed setup aid to be by player count, with images
		//3. Fixed boxy card image on player aid
		//4. Unknown! Maybe fonts? Maybe scaling? Who knows!

		private static bool useOverlay = false;

		//r = red = axe = wood
		//g = green = chisel = stone
		//k = black = sword = bone
		//w = white = staff = magic
		//u = blue = pick = iron

		private static readonly Dictionary<string, string> abbreviatedColorMap = new Dictionary<string, string>
		{
			{ "w", "Magic" },
			{ "k", "Dragonbone" },
			{ "g", "Stone" },
			{ "r", "Wood" },
			{ "u", "Iron" }
		};

		private static readonly Dictionary<string, string> colorMap = new Dictionary<string, string>
		{
			{ "white", "Magic" },
			{ "black", "Dragonbone" },
			{ "green", "Stone" },
			{ "red", "Wood" },
			{ "blue", "Iron" }
		};

		static void Main()
		{
			var imageCreator = new SplendorImageCreator();

			var newCards = ConvertCardsToNewCards();
			var toolFrontImages = newCards.Select((newCard, index) => imageCreator.CreateToolCardFront(newCard, index)).ToList();
			var toolBackImages = Enumerable.Range(1, 3).Select(index => imageCreator.CreateToolCardBack(index)).ToList();

			var playerAidFrontImage = new [] {imageCreator.CreatePlayerAidFront()};
			var playerAidBackImage = new [] {imageCreator.CreatePlayerAidBack()};

			var questFrontImages = QuestFactory.CreateQuests().Select(quest => imageCreator.CreateQuestFront(quest)).ToList();
			var questBackImage = new [] {imageCreator.CreateQuestBack()};

			var setupAidImage = new[] {imageCreator.CreateSetupAidFront()};
			var questAidImage = new[] { imageCreator.CreateQuestAidFront() };

			var allImages = toolFrontImages
				.Concat(toolBackImages)
				.Concat(playerAidFrontImage)
				.Concat(playerAidBackImage)
				.Concat(questFrontImages)
				.Concat(questAidImage)
				.Concat(questBackImage)
				.Concat(setupAidImage);

			var dateStamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
			Directory.CreateDirectory($"c:\\delete\\images\\{dateStamp}");

			if (useOverlay)
			{
				var overlay = new Bitmap("c:\\delete\\poker-card.png");
				overlay.SetResolution(300, 300);
				var landscapeOverlay = new Bitmap(overlay);
				landscapeOverlay.RotateFlip(RotateFlipType.Rotate90FlipNone);
				var matrix = new ColorMatrix { Matrix33 = .5f };
				var attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				foreach (var image in allImages)
				{
					var graphics = Graphics.FromImage(image.Bitmap);
					if (image.Bitmap.Width < image.Bitmap.Height)
					{
						graphics.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height), 0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel, attributes);
					}
					else
					{
						graphics.DrawImage(landscapeOverlay, new Rectangle(0, 0, landscapeOverlay.Width, landscapeOverlay.Height), 0, 0, landscapeOverlay.Width, landscapeOverlay.Height, GraphicsUnit.Pixel, attributes);
					}
				}
			}

			foreach (var image in allImages)
				image.Bitmap.Save($"c:\\delete\\images\\{dateStamp}\\{image.Name}.png", ImageFormat.Png);
		}

		private static IList<NewToolCard> ConvertCardsToNewCards()
		{
			var strings = File.ReadAllLines("input.txt");
			var cards = strings
				.Select(s => s.Split('\t'))
				.Select(tokens => new ToolCard
				{
					ResourceProduced = tokens[2],
					Costs = tokens[3].Split('+').ToDictionary(token => CharToString(token[1]), token => CharToInt(token[0])),
					Points = string.IsNullOrWhiteSpace(tokens[1]) ? 0 : int.Parse(tokens[1]),
					Tier = string.IsNullOrWhiteSpace(tokens[0]) ? 0 : int.Parse(tokens[0])
				})
				.ToList();

			var newCards = cards.Select(card =>
			{
				var transformedCosts = TransformCosts(card.Costs);
				return new NewToolCard
				{
					ResourceProduced = TransformColorProduced(card.ResourceProduced),
					Costs = transformedCosts,
					Points = card.Points,
					Tier = card.Tier
				};
			}).ToList();

			return newCards;
		}

		private static string CharToString(char input)
		{
			return new string(new [] {input});
		}

		private static int CharToInt(char input)
		{
			return int.Parse(CharToString(input));
		}

		private static IDictionary<string, int> TransformCosts(IDictionary<string, int> costs)
		{
			return costs.ToDictionary(colorCost => abbreviatedColorMap[colorCost.Key], colorCost => colorCost.Value);
		}

		private static string TransformColorProduced(string colorProduced)
		{
			return colorMap[colorProduced];
		}
	}
}
