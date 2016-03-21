using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Splendor
{
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
			var newCards = ConvertCardsToNewCards();
			var pdf = PdfCreator.CreatePdfDocument(newCards.Select(ImageCreator.CreateCardImage).ToList());
			pdf.Save("c:\\delete\\test.pdf");
		}

		private static IEnumerable<NewCard> ConvertCardsToNewCards()
		{
			var strings = File.ReadAllLines("c:\\delete\\input");
			var cards = strings
				.Select(s => s.Split('\t'))
				.Select(tokens => new Card
				{
					ResourceProduced = tokens[0],
					Costs = tokens[1].Split('+').ToDictionary(token => CharToString(token[1]), token => CharToInt(token[0]))
				})
				.ToList();

			var newCards = cards.Select(card =>
			{
				var transformedCosts = TransformCosts(card.Costs);
				return new NewCard
				{
					ResourceProduced = TransformColorProduced(card.ResourceProduced),
					Costs = transformedCosts
				};
			});

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
