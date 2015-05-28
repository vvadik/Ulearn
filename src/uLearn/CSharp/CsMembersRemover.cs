using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using uLearn.Lessions;

namespace uLearn.CSharp
{
	public class CsMembersRemover : IRegionRemover
	{
		private static bool Remove(Label label, ref SyntaxNode tree)
		{
			var members = tree.GetMembers()
				.Where(node => node.Identifier().ValueText == label.Name)
				.ToList();
			if (!members.Any())
				return false;
			if (label.OnlyBody)
				tree = tree.RemoveNodes(members.SelectMany(syntax => syntax.GetBody()), SyntaxRemoveOptions.KeepNoTrivia);
			else
				tree = tree.RemoveNodes(members, SyntaxRemoveOptions.KeepExteriorTrivia);
			return true;
		}

		public IEnumerable<Label> Remove(ref string code, IEnumerable<Label> labels)
		{
			var tree = CSharpSyntaxTree.ParseText(code).GetRoot();
			var res = labels.Where(label => !Remove(label, ref tree)).ToList();
			code = tree.ToFullString();
			return res;
		}

		public int RemoveSolution(ref string code, Label label)
		{
			var tree = CSharpSyntaxTree.ParseText(code).GetRoot();
			var solution = tree.GetMembers().FirstOrDefault(node => node.Identifier().ValueText == label.Name);
			if (solution == null)
				return -1;
			int res;
			if (label.OnlyBody)
			{
				var body = solution.GetBody();
				res = body.FullSpan.Start;
				tree = tree.RemoveNodes(body, SyntaxRemoveOptions.KeepNoTrivia);
			}
			else
			{
				res = solution.FullSpan.Start;
				tree = tree.RemoveNode(solution, SyntaxRemoveOptions.KeepExteriorTrivia);
			}
			const string pragma = "\r\n#line 1\r\n";
			code = tree.ToFullString().Insert(res, pragma);
			return res + pragma.Length;
		}

		public static string RemoveUsings(string code)
		{
			var tree = CSharpSyntaxTree.ParseText(code).GetRoot();
			var usings = tree.DescendantNodes().OfType<UsingDirectiveSyntax>();
			return tree.RemoveNodes(usings, SyntaxRemoveOptions.KeepExteriorTrivia).ToFullString();
		}
	}
}