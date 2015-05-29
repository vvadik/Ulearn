using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
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