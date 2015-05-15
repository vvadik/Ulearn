using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using uLearn.CSharp;

namespace uLearn
{
	public interface ISingleRegionExtractor
	{
		string GetRegion(IncludeCodeBlock.Label label);
	}

	public class RegionsExtractor
	{
		private readonly List<ISingleRegionExtractor> extractors;

		public RegionsExtractor(string code, string language)
		{
			extractors = new List<ISingleRegionExtractor>
			{
				new CommonSingleRegionExtractor(code)
			};
			if (language == "cs")
				extractors.Add(new CsMembersExtractor(code));
		}

		public string GetRegion(IncludeCodeBlock.Label label)
		{
			return extractors.Select(extractor => extractor.GetRegion(label)).FirstOrDefault(res => res != null);
		}

		public IEnumerable<string> GetRegions(IEnumerable<IncludeCodeBlock.Label> labels)
		{
			return labels.Select(GetRegion).Where(s => s != null).Select(FixEolns);
		}

		private static string FixEolns(string arg)
		{
			return Regex.Replace(arg.Trim(), "(\t*\r*\n){3,}", "\r\n\r\n");
		}
	}

	public class CsMembersExtractor : ISingleRegionExtractor
	{
		private readonly Dictionary<string, List<MemberDeclarationSyntax>> members;

		public CsMembersExtractor(string code)
		{
			var tree = CSharpSyntaxTree.ParseText(code);
			members = tree.GetRoot().DescendantNodes()
				.OfType<MemberDeclarationSyntax>()
				.Where(node => node is BaseTypeDeclarationSyntax || node is MethodDeclarationSyntax)
				.Where(node => !node.HasAttribute<HideOnSlideAttribute>())
				.Select(HideOnSlide)
				.GroupBy(node => node.Identifier().ValueText)
				.ToDictionary(
					nodes => nodes.Key,
					nodes => nodes.ToList()
				);
		}

		public string GetRegion(IncludeCodeBlock.Label label)
		{
			if (!members.ContainsKey(label.Name))
				return null;
			var nodes = members[label.Name];
			if (label.OnlyBody)
				return string.Join("\r\n\r\n", nodes.Select(GetBody));
			return String.Join("\r\n\r\n", nodes.Select(node => node.ToPrettyString()));
		}

		private static MemberDeclarationSyntax HideOnSlide(MemberDeclarationSyntax node)
		{
			var hide = node.DescendantNodes()
				.OfType<MemberDeclarationSyntax>()
				.Where(syntax => syntax.HasAttribute<HideOnSlideAttribute>());
			return node.RemoveNodes(hide, SyntaxRemoveOptions.KeepLeadingTrivia);
		}

		private static string GetBody(SyntaxNode node)
		{
			var method = node as BaseMethodDeclarationSyntax;
			if (method != null)
				return method.Body.Statements.ToFullString().RemoveCommonNesting();
			var type = node as TypeDeclarationSyntax;
			if (type != null)
				return type.Members.ToFullString().RemoveCommonNesting();
			return "";
		}
	}

	public class CommonSingleRegionExtractor : ISingleRegionExtractor
	{
		private readonly Dictionary<string, string> regions;

		public CommonSingleRegionExtractor(string code)
		{
			regions = new Dictionary<string, string>();

			var opened = new Dictionary<string, List<string>>();
			foreach (var line in code.SplitToLines())
			{
				if (line.Contains("endregion"))
				{
					var name = GetRegionName(line);
					if (opened.ContainsKey(name))
					{
						regions[name] = String.Join("\r\n", opened[name].RemoveCommonNesting());
						opened.Remove(name);
					}
				}

				foreach (var value in opened.Values)
					value.Add(line);

				if (line.Contains("region"))
				{
					var name = GetRegionName(line);
					opened[name] = new List<string>();
				}
			}
		}

		public string GetRegion(IncludeCodeBlock.Label label)
		{
			return regions.Get(label.Name, null);
		}

		private static string GetRegionName(string line)
		{
			return line.Split().Last();
		}
	}
}