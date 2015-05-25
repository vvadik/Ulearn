using System;
using System.Collections.Generic;
using System.Linq;

namespace uLearn
{
	public static class RegionsParser
	{
		public class Region
		{
			public readonly int dataStart;
			public readonly int dataLength;
			public readonly int fullStart;
			public readonly int fullLength;

			public Region(int dataStart, int dataLength, int fullStart, int fullLength)
			{
				this.dataStart = dataStart;
				this.dataLength = dataLength;
				this.fullStart = fullStart;
				this.fullLength = fullLength;
			}
		}

		public static Dictionary<string, Region> GetRegions(string code)
		{
			var regions = new Dictionary<string, Region>();
			var opened = new Dictionary<string, Tuple<int, int>>();
			var current = 0;

			foreach (var line in code.SplitToLinesWithEoln())
			{
				if (line.Contains("endregion"))
				{
					var name = GetRegionName(line);
					if (opened.ContainsKey(name))
					{
						var start = opened[name];
						regions[name] = new Region(start.Item1, current - start.Item1, start.Item2, current - start.Item2 + line.Length);
						opened.Remove(name);
					}
					current += line.Length;
					continue;
				}

				current += line.Length;
				
				if (line.Contains("region"))
				{
					var name = GetRegionName(line);
					opened[name] = Tuple.Create(current, current - line.Length);
				}
			}

			return regions;
		}

		private static string GetRegionName(string line)
		{
			return line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).Last();
		}
	}
}