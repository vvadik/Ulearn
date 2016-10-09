using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public abstract class BaseStyleValidator : ICSharpSolutionValidator
	{
		protected string Report(SyntaxNode syntaxNode, string message)
		{
			return Report(syntaxNode.SyntaxTree.GetLineSpan(syntaxNode.Span), message);
		}

		protected string Report(SyntaxToken syntaxToken, string message)
		{
			return Report(syntaxToken.SyntaxTree.GetLineSpan(syntaxToken.Span), message);
		}

		private static string Report(FileLinePositionSpan span, string message)
		{
			var linePosition = span.StartLinePosition;
			return "Строка {0}: {1}".WithArgs(linePosition.Line + 1, message);
		}

		protected IEnumerable<string> InspectAll<TNode>(SyntaxTree userSolution, Func<TNode, IEnumerable<string>> inspect)
		{
			var nodes = userSolution.GetRoot().DescendantNodes().OfType<TNode>();
			return nodes.SelectMany(inspect);
		}

		public string FindError(string code)
		{
			return FindError(CSharpSyntaxTree.ParseText(code));
		}

		public string FindError(SyntaxTree userSolution)
		{
			var errors = ReportAllErrors(userSolution).ToList();
			return errors.Any() ? string.Join("\n", errors) : null;
		}

		protected abstract IEnumerable<string> ReportAllErrors(SyntaxTree userSolution);
	}
}