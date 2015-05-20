using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using uLearn.CSharp;

namespace uLearn
{
	public interface IRegionRemover
	{
		IEnumerable<IncludeCodeBlock.Label> Remove(ref string code, IEnumerable<IncludeCodeBlock.Label> labels);
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

		public IEnumerable<IncludeCodeBlock.Label> Remove(ref string code, IEnumerable<IncludeCodeBlock.Label> labels)
		{
			foreach (var regionRemover in regionRemovers)
			{
				labels = regionRemover.Remove(ref code, labels);
			}
			return labels.ToList();
		}
	}

	public class CsMembersRemover : IRegionRemover
	{
		private static bool Remove(IncludeCodeBlock.Label label, ref SyntaxNode tree)
		{
			var members = tree.GetMembers()
				.Where(node => node.Identifier().ValueText == label.Name)
				.ToList();
			if (!members.Any())
				return false;
			tree = tree.RemoveNodes(label.OnlyBody ? members.SelectMany(syntax => syntax.GetBody()) : members, SyntaxRemoveOptions.KeepNoTrivia);
			return true;
		}

		public IEnumerable<IncludeCodeBlock.Label> Remove(ref string code, IEnumerable<IncludeCodeBlock.Label> labels)
		{
			var tree = CSharpSyntaxTree.ParseText(code).GetRoot();
			var res = labels.Where(label => !Remove(label, ref tree)).ToList();
			code = tree.ToFullString();
			return res;
		}
	}

	public class CommonRegionRemover : IRegionRemover
	{
		public IEnumerable<IncludeCodeBlock.Label> Remove(ref string code, IEnumerable<IncludeCodeBlock.Label> labels)
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
	}
}