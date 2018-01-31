using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Extensions;
using uLearn.Model.Blocks;
using Ulearn.Common.Extensions;

namespace uLearn.Model
{
	public class CommonRegionRemover : IRegionRemover
	{
		public string Remove(string code, IEnumerable<Label> labels, out IEnumerable<Label> notRemoved)
		{
			var regions = RegionsParser.GetRegions(code);

			var labelsList = labels.ToList();
			var blocks = labelsList.Select(label => regions.GetOrDefault(label.Name, null)).Where(region => region != null).OrderByDescending(region => region.fullStart + region.fullLength);
			var prevStart = int.MaxValue;

			foreach (var region in blocks)
			{
				if (region.fullStart >= prevStart)
					continue;
				code = code.Remove(region.fullStart, Math.Min(region.fullLength, prevStart - region.fullStart));
				prevStart = region.fullStart;
			}

			notRemoved = labelsList.Where(label => !regions.ContainsKey(label.Name)).ToList();
			return code;
		}

		public string RemoveSolution(string code, Label label, out int index)
		{
			var regions = RegionsParser.GetRegions(code);
			if (!regions.ContainsKey(label.Name))
			{
				index = -1;
				return code;
			}

			var region = regions[label.Name];
			index = region.fullStart;
			return code.Remove(region.fullStart, region.fullLength);
		}

		public string Prepare(string code)
		{
			return code;
		}
	}
}