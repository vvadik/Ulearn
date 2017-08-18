using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn
{
	public class IndentsValidator : CSharpSyntaxWalker
	{
		private readonly SyntaxTree tree;
		private readonly SyntaxTrivia nestingLeadingTrivia;
		private SyntaxNode root => tree.GetRoot();

		public IndentsValidator(string code, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
			: base(depth)
		{
			tree = CSharpSyntaxTree.ParseText(code);
			nestingLeadingTrivia = root.GetLeadingTrivia().LastOrDefault();
		}

		public void ReportFirstWarningIfIndentsNotValidated()
		{
			Visit(tree.GetRoot());
		}

		public override void Visit(SyntaxNode node)
		{
			if (ReportIfCurrentNodeDoesntHaveNestingLeadingTrivia(node))
				return;
			base.Visit(node);
		}

		private bool ReportIfCurrentNodeDoesntHaveNestingLeadingTrivia(SyntaxNode node)
		{
			if (node.IsKind(SyntaxKind.CompilationUnit) ||
				node.IsKind(SyntaxKind.IdentifierName))
				return false;
			var currentNodeLeadingTrivia = node.GetLeadingTrivia();
			if (currentNodeLeadingTrivia.LastOrDefault() != nestingLeadingTrivia)
			{
				var nestingIndentline = tree.GetLineSpan(root.Span).Span.Start.Line;
				var wrongIndentline = tree.GetLineSpan(node.Span).Span.Start.Line;
				Warning?.Invoke($"Line {wrongIndentline} has no nesting indent as at line {nestingIndentline}");
				return true;
			}
			return false;
		}

		public event Action<string> Warning;
	}
}