using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp;
using PdfSharp.Pdf;

namespace Splendor
{
	class Program
	{
		//TODO: Change to one image per card
		//TODO: Move tool icons/points/resource produced to top of card
		//TODO: Fix borders (add bleed areas, etc, make line up right with back)
		//TODO: thicker card stock
		//TODO: Fix font size (servan)
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
			var pdfDocument = new PdfDocument();
			var imageCreator = new ImageCreator();

			var newCards = ConvertCardsToNewCards();
			var cardsGroupedByTier = newCards.GroupBy(newCard => newCard.Tier);
			foreach (var cardGroup in cardsGroupedByTier)
			{
				var remainingCards = cardGroup.ToList();
				while (remainingCards.Any())
				{
					var imagesToUse = remainingCards.Take(9);
					PdfCreator.AddPageToPdf(pdfDocument, imagesToUse.Select(imageCreator.CreateToolCardFront).ToList(), PageOrientation.Portrait);
					PdfCreator.AddPageToPdf(pdfDocument, Enumerable.Range(0, 9).Select(index => imageCreator.CreateToolCardBack(cardGroup.Key)).ToList(), PageOrientation.Portrait);
					remainingCards = remainingCards.Skip(9).ToList();
				}
			}

			PdfCreator.AddPageToPdf(pdfDocument, Enumerable.Range(0, 9).Select(index => imageCreator.CreatePlayerAidFront()).ToList(), PageOrientation.Landscape);

			PdfCreator.AddPageToPdf(pdfDocument, Enumerable.Range(0, 9).Select(index => imageCreator.CreatePlayerAidBack()).ToList(), PageOrientation.Landscape);

			var questFrontImages = QuestFactory.CreateQuests().Select(quest => imageCreator.CreateQuestFront(quest)).Concat(new[] { imageCreator.CreateQuestAidFront() }).ToList();
			var remainingQuestImages = questFrontImages.ToList();
			while (remainingQuestImages.Any())
				{
					var imagesToUse = remainingQuestImages.Take(9);
					PdfCreator.AddPageToPdf(pdfDocument, imagesToUse.ToList(), PageOrientation.Portrait);
					PdfCreator.AddPageToPdf(pdfDocument, Enumerable.Range(0, 9).Select(index => imageCreator.CreateQuestBack()).ToList(), PageOrientation.Portrait);
					remainingQuestImages = remainingQuestImages.Skip(9).ToList();
				}

			var path = "c:\\delete\\Splendor Cards.pdf";
			pdfDocument.Save(path);
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
