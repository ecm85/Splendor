using System;
using System.Collections.Generic;
using System.Linq;

namespace Splendor
{
	public class NewCard : Card
	{
		private static readonly Dictionary<string, string> materialsByResource = new Dictionary<string, string>
		{
			{ "Iron", "Iron" },
			{ "Dragonbone", "Dragonbone" },
			{ "Wood", "Wooden" },
			{ "Stone", "Stone" },
			{ "Magic", "Magic" }
		};

		private static readonly Dictionary<Tuple<string, string>, string>  doubleMaterialsByResources = new Dictionary<Tuple<string, string>, string>
		{
			{Tuple.Create("Iron", "Wood"), "Ironwood"},
			{Tuple.Create("Iron", "Stone"), "Ironstone"},
			{Tuple.Create("Iron", "Dragonbone"), "Ironbone"},
			{Tuple.Create("Dragonbone", "Wood"), "Dragonwood"},
			{Tuple.Create("Dragonbone", "Stone"), "Dragonstone"},
			{Tuple.Create("Wood", "Stone"), "Woodstone"},
			{Tuple.Create("Magic", "Iron"), "Enchanted Iron"},
			{Tuple.Create("Magic", "Stone"), "Enchanted Stone"},
			{Tuple.Create("Magic", "Wood"), "Enchanted Wooden"},
			{Tuple.Create("Magic", "Dragonbone"), "Enchanted Dragonbone"}
		};

		private static readonly Dictionary<string, string> toolsByResourceProduced = new Dictionary<string, string>
		{
			{"Wood", "Axe" },
			{"Dragonbone", "Sword" },
			{"Stone", "Chisel" },
			//{"Stone", "Hammer & Chisel" },
			{"Iron", "Pick" },
			{"Magic", "Staff" }
		};

		public string Name => $"{Quality} {Material} {Tool}";

		private string Tool => toolsByResourceProduced[ResourceProduced];

		private string Material
		{
			get
			{
				switch (Costs.Count)
				{
					case 1:
						return materialsByResource[Costs.Keys.Single()];
					case 2:
						var materials = Costs.Keys.ToList();
						var firstDirection = Tuple.Create(materials[0], materials[1]);
						var secondDirection = Tuple.Create(materials[1], materials[0]);
						return doubleMaterialsByResources.ContainsKey(firstDirection) ?
							doubleMaterialsByResources[firstDirection] :
							doubleMaterialsByResources[secondDirection];
					case 3:
						var highestCount = Costs.Values.Max();
						return Costs.Values.Any(cost => cost <= highestCount - 2) ?
							materialsByResource[Costs.Keys.Single(key => Costs[key] == highestCount)] :
							"Composite";
					case 4:
						return "Composite";
				}
				return null;
			}
		}

		private string Quality
		{
			get
			{
				var costs = Costs.Values.ToList();
				switch (costs.Count)
				{
					case 4:
						return costs.Max() < 3 ? "Shoddy" : "Strong";
					case 3:
						return costs.Sum() == 5
							? (costs.Max() < 3 ? "Shoddy" : "Shoddy Braced")
							: (costs.Max() < 4 ? "Standard" : (costs.Max() < 6 ? "Durable Braced" : "Indestructible Braced"));
					case 2:
						return costs.Max() < 3 ? "Shoddy" : (costs.Max() < 6 ? "Durable" : "Godly");
					case 1:
						switch (costs.Single())
						{
							case 3:
								return "Shoddy";
							case 4:
								return "Standard";
							case 5:
								return "Durable";
							case 6:
								return "Strong";
							case 7:
								return "Indestructible";
							default:
								return null;
						}
				}
				return "Shoddy";
			}
		}
	}
}
