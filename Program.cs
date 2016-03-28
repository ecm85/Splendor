using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Splendor
{
	class Program
	{
		//TODO: Fix borders (add bleed areas, etc, make line up right with back)
		//TODO: thicker card stock
		//TODO: Remove setup from quest aid and add message about only 1 per turn.
		//TODO: Add a full setup player aid card
		//TODO: Change quest back color?
		//TODO: Make quest aid double-sided? Or put full setup on it

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
			var imageCreator = new ImageCreator();

			var newCards = ConvertCardsToNewCards();
			var toolFrontImages = newCards.Select(newCard => new ImageToSave {Image = imageCreator.CreateToolCardFront(newCard), Name = $"Tier {newCard.Tier} Front - {newCard.Name}"});
			var toolBackImages = Enumerable.Range(1, 3).Select(index => new ImageToSave {Image = imageCreator.CreateToolCardBack(index), Name = $"Tier {index} Back"});
			var playerAidFrontImage = new [] {new ImageToSave {Image = imageCreator.CreatePlayerAidFront(), Name = "Player Aid Front"}};
			var playerAidBackImage = new [] {new ImageToSave {Image = imageCreator.CreatePlayerAidBack(), Name = "Player Aid Back"}};
			var questFrontImages = QuestFactory.CreateQuests().Select(quest => new ImageToSave {Image = imageCreator.CreateQuestFront(quest), Name = $"Quest - {quest.Name}"});
			var questAidImage = new [] {new ImageToSave {Image = imageCreator.CreateQuestAidFront(), Name = "Quest Aid"}};
			var questBackImage = new [] {new ImageToSave {Image = imageCreator.CreateQuestBack(), Name = "Quest Back"}};

			var allImages = toolFrontImages
				.Concat(toolBackImages)
				.Concat(playerAidFrontImage)
				.Concat(playerAidBackImage)
				.Concat(questFrontImages)
				.Concat(questAidImage)
				.Concat(questBackImage);

			foreach (var image in allImages)
			{
				image.Image.Save($"c:\\delete\\images\\{image.Name}.png", ImageFormat.Png);
			}
		}

		private static IList<NewCard> ConvertCardsToNewCards()
		{
			var strings = File.ReadAllLines("c:\\delete\\input");
			var cards = strings
				.Select(s => s.Split('\t'))
				.Select(tokens => new Card
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
				return new NewCard
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
