using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	public abstract class BaseStyleValidator : ICSharpSolutionValidator
	{
		protected FileLinePositionSpan GetSpan(SyntaxNode syntaxNode)
		{
			return syntaxNode.SyntaxTree.GetLineSpan(syntaxNode.Span);
		}

		protected FileLinePositionSpan GetSpan(SyntaxToken syntaxNode)
		{
			return syntaxNode.SyntaxTree.GetLineSpan(syntaxNode.Span);
		}

		protected string Report(SyntaxNode syntaxNode, string message)
		{
			return Report(GetSpan(syntaxNode), message);
		}

		protected string Report(SyntaxToken syntaxToken, string message)
		{
			return Report(GetSpan(syntaxToken), message);
		}

		public static string Report(FileLinePositionSpan span, string message)
		{
			var linePosition = span.StartLinePosition;
			return "Строка {0}, позиция {1}: {2}".WithArgs(linePosition.Line + 1, linePosition.Character, message);
		}

		protected IEnumerable<string> InspectAll<TNode>(SyntaxTree userSolution, Func<TNode, IEnumerable<string>> inspect)
			where TNode : SyntaxNode
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