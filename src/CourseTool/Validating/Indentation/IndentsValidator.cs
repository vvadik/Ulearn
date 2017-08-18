using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn
{
	public class IndentsValidator : CSharpSyntaxWalker
	{
		public void ReportWarningIfIndentsNotValidated(string code)
		{
			var tree = CSharpSyntaxTree.ParseText(code);
			Visit(tree.GetRoot());
		}

		public override void Visit(SyntaxNode node)
		{
			Console.WriteLine(node);
			Console.WriteLine();
			base.Visit(node);
		}

		public override void VisitLeadingTrivia(SyntaxToken token)
		{
			//Console.WriteLine(token.GetAllTrivia().Select(t => $"{t.ToString()} "));
			token.GetAllTrivia().Last().IsKind(SyntaxKind.WhitespaceTrivia);
			base.VisitLeadingTrivia(token);
		}

		public event Action<string> Warning;
	}
}