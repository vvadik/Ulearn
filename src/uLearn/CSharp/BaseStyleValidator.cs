using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SyntaxNodeOrToken = uLearn.CSharp.IndentsValidation.SyntaxNodeOrToken;

namespace uLearn.CSharp
{
	public abstract class BaseStyleValidator : ICSharpSolutionValidator
	{
		public static FileLinePositionSpan GetSpan(SyntaxNode syntaxNode)
		{
			return syntaxNode.SyntaxTree.GetLineSpan(syntaxNode.Span);
		}

		public static FileLinePositionSpan GetSpan(SyntaxToken syntaxNode)
		{
			return syntaxNode.SyntaxTree.GetLineSpan(syntaxNode.Span);
		}

		public static FileLinePositionSpan GetSpan(SyntaxNodeOrToken syntaxNode)
		{
			return syntaxNode.GetFileLinePositionSpan();
		}

		public static string Report(SyntaxNodeOrToken syntaxNode, string message)
		{
			return Report(GetSpan(syntaxNode), message);
		}

		public static string Report(SyntaxNode syntaxNode, string message)
		{
			return Report(GetSpan(syntaxNode), message);
		}

		public static string Report(SyntaxToken syntaxToken, string message)
		{
			return Report(GetSpan(syntaxToken), message);
		}

		public static string Report(FileLinePositionSpan span, string message)
		{
			var linePosition = span.StartLinePosition;
			return "Строка {0}, позиция {1}: {2}".WithArgs(linePosition.Line + 1, linePosition.Character, message);
		}

		public static IEnumerable<string> InspectAll<TNode>(SyntaxTree userSolution, Func<TNode, IEnumerable<string>> inspect)
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