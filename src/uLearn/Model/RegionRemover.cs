using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Model
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
			code = code.FixExtraEolns();
			return labels.ToList();
		}

		public int RemoveSolution(ref string code, Label label)
		{
			foreach (var regionRemover in regionRemovers)
			{
				var pos = regionRemover.RemoveSolution(ref code, label);
				if (pos < 0)
					continue;
				code = code.FixExtraEolns();
				return pos;
			}
			return -1;
		}
	}
}