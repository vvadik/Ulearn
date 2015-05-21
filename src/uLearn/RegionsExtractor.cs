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
			members = tree.GetRoot().GetMembers()
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
			return node.GetBody().ToFullString().RemoveCommonNesting();
		}
	}

	public class CommonSingleRegionExtractor : ISingleRegionExtractor
	{
		private readonly Dictionary<string, RegionsParser.Region> regions;
		private readonly string code;

		public CommonSingleRegionExtractor(string code)
		{
			this.code = code;
			regions = RegionsParser.GetRegions(code);
		}

		public string GetRegion(IncludeCodeBlock.Label label)
		{
			var region = regions.Get(label.Name, null);
			if (region == null)
				return null;
			return code.Substring(region.dataStart, region.dataLength).RemoveCommonNesting();
		}
	}

	public static class SyntaxNodeExtentions
	{
		public static IEnumerable<MemberDeclarationSyntax> GetMembers(this SyntaxNode node)
		{
			return node.DescendantNodes()
				.OfType<MemberDeclarationSyntax>()
				.Where(n => n is BaseTypeDeclarationSyntax || n is MethodDeclarationSyntax);
		}

		public static SyntaxList<SyntaxNode> GetBody(this SyntaxNode node)
		{
			var method = node as BaseMethodDeclarationSyntax;
			if (method != null)
			{
				var body = method.Body;
				if (body != null)
					return body.Statements;
				return new SyntaxList<SyntaxNode>();
			}
			var type = node as TypeDeclarationSyntax;
			if (type != null)
				return type.Members;
			return new SyntaxList<SyntaxNode>();
		}
	}

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