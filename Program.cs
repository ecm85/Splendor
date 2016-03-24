using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PdfSharp;

namespace Splendor
{
	//TODO: Quest cards
	//TODO: Quest aid
	//			Quest aid:
	//				side 1: message about quests
	//TODO: Backpack w/ 10 slots?
	//TODO: Tweak formatting on player aid side 1

	class Program
	{
		private static readonly Dictionary<string, string> abbreviatedColorMap = new Dictionary<string, string>
		{
			{ "w", "Iron" },
			{ "k", "Dragonbone" },
			{ "g", "Wood" },
			{ "r", "Magic" },
			{ "u", "Stone" }
		};

		private static readonly Dictionary<string, string> colorMap = new Dictionary<string, string>
		{
			{ "white", "Iron" },
			{ "black", "Dragonbone" },
			{ "green", "Wood" },
			{ "red", "Magic" },
			{ "blue", "Stone" }
		};

		static void Main()
		{
			var paths = new List<string>();
			var imageCreator = new ImageCreator();
			var newCards = ConvertCardsToNewCards();
			var cardsGroupedByTier = newCards.GroupBy(newCard => newCard.Tier);
			foreach (var cardGroup in cardsGroupedByTier)
			{
				var cardPdf = PdfCreator.CreatePdfDocument(cardGroup.Select(imageCreator.CreateCardImage).ToList(), PageOrientation.Portrait);
				var cardPath = $"c:\\delete\\Splendor Tier {cardGroup.Key} Cards.pdf";
				cardPdf.Save(cardPath);
				paths.Add(cardPath);
				var cardBackPdf = PdfCreator.CreatePdfDocument(Enumerable.Range(0, 9).Select(index => imageCreator.CreateCardBackImage(cardGroup.Key)).ToList(), PageOrientation.Portrait);
				var cardBackPath = $"c:\\delete\\Splendor Tier {cardGroup.Key} Card Backs.pdf";
				cardBackPdf.Save(cardBackPath);
				paths.Add(cardBackPath);
			}
			var playerAidFrontPdf = PdfCreator.CreatePdfDocument(Enumerable.Range(0, 9).Select(index => imageCreator.CreatePlayerAidFront()).ToList(), PageOrientation.Landscape);
			var playerAidFrontPath = "c:\\delete\\player aid front.pdf";
			playerAidFrontPdf.Save(playerAidFrontPath);
			paths.Add(playerAidFrontPath);
			var playerAidBackPdf = PdfCreator.CreatePdfDocument(Enumerable.Range(0, 9).Select(index => imageCreator.CreatePlayerAidBack()).ToList(), PageOrientation.Landscape);
			var playerAidBackPath = "c:\\delete\\player aid back.pdf";
			playerAidBackPdf.Save(playerAidBackPath);
			paths.Add(playerAidBackPath);

			foreach (var path in paths)
				Process.Start(path);
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
