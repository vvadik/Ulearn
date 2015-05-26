using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn
{
	public interface IRegionRemover
	{
		IEnumerable<Label> Remove(ref string code, IEnumerable<Label> labels);
		int RemoveSolution(ref string code, Label label);
	}

	public class RegionRemover : IRegionRemover
	{
		private readonly List<IRegionRemover> regionRemovers = new List<IRegionRemover>();

		public RegionRemover(string language)
		{
			if (language == "cs")
				regionRemovers.Add(new CsMembersRemover());
			regionRemovers.Add(new CommonRegionRemover());
		}

		public IEnumerable<Label> Remove(ref string code, IEnumerable<Label> labels)
		{
			foreach (var regionRemover in regionRemovers)
			{
				labels = regionRemover.Remove(ref code, labels);
			}
			return labels.ToList();
		}

		public int RemoveSolution(ref string code, Label label)
		{
			foreach (var regionRemover in regionRemovers)
			{
				var pos = regionRemover.RemoveSolution(ref code, label);
				if (pos >= 0)
					return pos;
			}
			return -1;
		}
	}

	public class CommonRegionRemover : IRegionRemover
	{
		public IEnumerable<Label> Remove(ref string code, IEnumerable<Label> labels)
		{
			var regions = RegionsParser.GetRegions(code);

			var labelsList = labels.ToList();
			var blocks = labelsList.Select(label => regions.Get(label.Name, null)).Where(region => region != null).OrderByDescending(region => region.fullStart + region.fullLength);
			var prevStart = int.MaxValue;

			foreach (var region in blocks)
			{
				if (region.fullStart >= prevStart) continue;
				code = code.Remove(region.fullStart, Math.Min(region.fullLength, prevStart - region.fullStart));
				prevStart = region.fullStart;
			}

			return labelsList.Where(label => !regions.ContainsKey(label.Name)).ToList();
		}

		public int RemoveSolution(ref string code, Label label)
		{
			var regions = RegionsParser.GetRegions(code);
			if (!regions.ContainsKey(label.Name))
				return -1;
			var region = regions[label.Name];
			code = code.Remove(region.fullStart, region.fullLength);
			return region.fullStart;
		}
	}
}