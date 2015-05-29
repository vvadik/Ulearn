using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
	public interface IRegionRemover
	{
		string Remove(string code, IEnumerable<Label> labels, out IEnumerable<Label> notRemoved);
		string RemoveSolution(string code, Label label, out int index);
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

		public string Remove(string code, IEnumerable<Label> labels, out IEnumerable<Label> notRemoved)
		{
			foreach (var regionRemover in regionRemovers)
			{
				code = regionRemover.Remove(code, labels, out notRemoved);
				labels = notRemoved;
			}
			notRemoved = labels.ToList();
			return code.FixExtraEolns();
		}

		public string RemoveSolution(string code, Label label, out int index)
		{
			foreach (var regionRemover in regionRemovers)
			{
				regionRemover.RemoveSolution(code, label, out index);
				if (index < 0)
					continue;
				return code.FixExtraEolns();
			}
			index = -1;
			return code;
		}
	}
}