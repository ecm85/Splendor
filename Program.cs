using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PdfSharp.Pdf;

namespace Splendor
{
	//TODO: Remove watermark from all images
	//TODO: Change resources to all be transparent backgrounds, and/or add my own standard one
	//TODO: Add card backs
	//TODO: Quest cards
	//TODO: Player aids and Quest aid
	//TODO: Shrink tool picture and place in hole?
	//TODO: Move/change icon?
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
			var imageCreator = new ImageCreator();
			var newCards = ConvertCardsToNewCards();
			var cardsGroupedByTier = newCards.GroupBy(newCard => newCard.Tier);
			var paths = new List<string>();
			foreach (var cardGroup in cardsGroupedByTier)
			{
				var pdf = PdfCreator.CreatePdfDocument(cardGroup.Select(imageCreator.CreateCardImage).ToList());
				var path = $"c:\\delete\\Splendor Tier {cardGroup.Key} Cards.pdf";
				pdf.Save(path);
				paths.Add(path);
			}
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

			//var mostWords = "";
			//var mostWordCount = 0;
			//foreach (var newCard in newCards)
			//{
			//	Console.WriteLine(newCard.Name);
			//	var tokens = newCard.Name.Split(' ');
			//	if (tokens.Length > mostWordCount)
			//	{
			//		mostWordCount = tokens.Length;
			//		mostWords = newCard.Name;
			//	}
			//}
			//Console.WriteLine($"Most words: {mostWordCount}, in: {mostWords}");

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
