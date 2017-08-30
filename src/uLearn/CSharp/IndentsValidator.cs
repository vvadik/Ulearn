using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using uLearn.CSharp;

namespace uLearn
{
	public class IndentsValidator : BaseStyleValidator
	{
		private const string prefix = "Код плохо отформатирован. " +
									"Автоматически отформатировать код можно в Visual Studio с помощью комбинации клавиш Ctrl + K + D";

		private SyntaxTree tree;
		private BracesPair[] bracesPairs;

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			tree = userSolution;
			bracesPairs = BuildBracesPairs().OrderBy(p => p.Open.SpanStart).ToArray();

			var errors = ReportIfCompilationUnitChildrenIndented()
				.Concat(ReportIfBracesNotAligned())
				.Concat(ReportIfOpenBraceHasCodeOnSameLine())
				.Concat(ReportIfBracesContentNotIndentedOrNotConsistent())
				.Concat(ReportIfBracesNotIndented())
				.ToArray();
			return errors.Any() ? new[] { prefix }.Concat(errors) : Enumerable.Empty<string>();
		}

		private IEnumerable<BracesPair> BuildBracesPairs()
		{
			var braces = tree.GetRoot().DescendantTokens()
				.Where(t => t.IsKind(SyntaxKind.OpenBraceToken) || t.IsKind(SyntaxKind.CloseBraceToken));
			var openbracesStack = new Stack<SyntaxToken>();
			foreach (var brace in braces)
			{
				if (brace.IsKind(SyntaxKind.OpenBraceToken))
					openbracesStack.Push(brace);
				else
					yield return new BracesPair(openbracesStack.Pop(), brace);
			}
		}

		private IEnumerable<string> ReportIfCompilationUnitChildrenIndented()
		{
			foreach (var childNode in tree.GetRoot().ChildNodes())
				if (Indent.Exists(childNode))
					yield return Report(childNode, "Лишний отступ.");
		}

		private IEnumerable<string> ReportIfBracesNotAligned()
		{
			foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
			{
				var openBraceIndent = new Indent(braces.Open);
				var closeBraceIndent = new Indent(braces.Close);
				if (openBraceIndent.IndentedTokenIsFirstAtLine &&
					!closeBraceIndent.IndentedTokenIsFirstAtLine ||
					openBraceIndent.LengthInSpaces != closeBraceIndent.LengthInSpaces)
				{
					yield return Report(braces.Open, $"Парные фигурные скобки ({braces}) должны иметь одинаковый отступ.");
				}
			}
		}

		private IEnumerable<string> ReportIfOpenBraceHasCodeOnSameLine()
		{
			foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
			{
				var openBraceHasCodeOnSameLine = braces.Open.Parent.ChildNodes()
					.Select(node => node.DescendantTokens().First())
					.Any(t => braces.TokenInsideBraces(t) && t.GetLine() == braces.Open.GetLine());
				if (openBraceHasCodeOnSameLine)
					yield return Report(braces.Open, "После открывающей фигурной скобки на той же строке не должно быть кода");
			}
		}

		private IEnumerable<string> ReportIfBracesContentNotIndentedOrNotConsistent()
		{
			foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
			{
				var childLineIndents = braces.Open.Parent.ChildNodes()
					.Select(node => node.DescendantTokens().First())
					.Where(t => braces.TokenInsideBraces(t))
					.Select(t => new Indent(t))
					.Where(i => i.IndentedTokenIsFirstAtLine)
					.ToList();
				if (!childLineIndents.Any())
					continue;
				var openbraceLineIndent = new Indent(braces.Open);
				var firstChild = childLineIndents.First();
				if (firstChild.LengthInSpaces <= openbraceLineIndent.LengthInSpaces)
					yield return Report(firstChild.IndentedToken,
						$"Содержимое парных фигурных скобок ({braces}) должно иметь дополнительный отступ.");
				var badLines = childLineIndents.Where(t => t.LengthInSpaces != firstChild.LengthInSpaces);
				foreach (var badIndent in badLines)
				{
					yield return Report(badIndent.IndentedToken,
						$"Содержимое парных фигурных скобок ({braces}) должно иметь одинаковый отступ.");
				}
			}
		}

		private IEnumerable<string> ReportIfBracesNotIndented()
		{
			foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine() &&
															Indent.TokenIsFirstAtLine(pair.Open)))
			{
				var parent = FindClosestParentOnAnotherLine(braces.Open);
				var parentLineIndent = new Indent(parent.DescendantTokens().First());
				var openbraceLineIndent = new Indent(braces.Open);
				if (openbraceLineIndent.LengthInSpaces < parentLineIndent.LengthInSpaces)
					yield return Report(braces.Open,
						$"Парные фигурные скобки ({braces}) должны иметь отступ не меньше, чем у родителя.");
			}
		}

		private SyntaxNode FindClosestParentOnAnotherLine(SyntaxToken child)
		{
			var parentFirstToken = child.Parent;
			while (parentFirstToken.DescendantTokens().First().GetLine() == child.GetLine())
			{
				parentFirstToken = parentFirstToken.Parent;
			}
			return parentFirstToken;
		}
	}

	internal class BracesPair
	{
		public readonly SyntaxToken Open;
		public readonly SyntaxToken Close;

		public BracesPair(SyntaxToken open, SyntaxToken close)
		{
			Open = open;
			Close = close;
		}

		public bool TokenInsideBraces(SyntaxToken token)
		{
			return token.SpanStart > Open.SpanStart && token.Span.End < Close.Span.End;
		}

		public override string ToString()
		{
			return $"строки {Open.GetLine() + 1}, {Close.GetLine() + 1}";
		}
	}

	internal class Indent
	{
		public int LengthInSpaces { get; }
		public bool IndentedTokenIsFirstAtLine { get; }
		public SyntaxToken IndentedToken { get; }

		public Indent(SyntaxToken indentedToken)
		{
			IndentedToken = indentedToken;
			IndentedTokenIsFirstAtLine = TokenIsFirstAtLine(indentedToken);
			var firstTokenInLine = GetFirstTokenAtLine(indentedToken);
			LengthInSpaces = GetLengthInSpaces(firstTokenInLine);
		}

		private int GetLengthInSpaces(SyntaxToken firstTokenInLine)
		{
			var lastTrivia = firstTokenInLine.LeadingTrivia.LastOrDefault();
			if (!lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
				return 0;
			var stringTrivia = lastTrivia.ToFullString();
			var type = GetIndentType(stringTrivia);
			if (type == IndentType.Whitespace)
				return stringTrivia.Length;
			if (type == IndentType.Tab)
				return stringTrivia.Length * 4;
			return stringTrivia.Count(c => c == ' ') + stringTrivia.Count(c => c == '\t') * 4;
		}

		public static IndentType GetIndentType(string leadingTrivia)
		{
			if (leadingTrivia.All(v => v == '\t'))
				return IndentType.Tab;
			if (leadingTrivia.All(v => v == ' '))
				return IndentType.Whitespace;
			return IndentType.Mixed;
		}

		public static bool Exists(SyntaxNodeOrToken nodeOrToken)
		{
			return nodeOrToken.GetLeadingTrivia().LastOrDefault().IsKind(SyntaxKind.WhitespaceTrivia);
		}

		public static SyntaxToken GetFirstTokenAtLine(SyntaxToken token)
		{
			var tokenLineSpanStart = token.SyntaxTree.GetText().Lines[token.GetLine()].Start;
			return token.SyntaxTree.GetRoot().FindToken(tokenLineSpanStart);
		}

		public static bool TokenIsFirstAtLine(SyntaxToken token)
		{
			return GetFirstTokenAtLine(token).IsEquivalentTo(token);
		}

		public override string ToString()
		{
			return $"{IndentedToken.Kind()}. LengthInSpaces: {LengthInSpaces}";
		}
	}

	internal enum IndentType
	{
		Tab,
		Whitespace,
		Mixed
	}

	public static class SyntaxTokenExt
	{
		public static int GetLine(this SyntaxToken t)
		{
			return t.GetLocation().GetLineSpan().StartLinePosition.Line;
		}
	}
}