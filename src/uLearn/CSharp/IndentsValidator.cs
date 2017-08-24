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
				.Concat(ReportIfBracesContentNotIndentedOrNotConsistent())
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
			foreach (var braces in bracesPairs)
			{
				var openBraceIndent = new Indent(braces.Open);
				var closeBraceIndent = new Indent(braces.Close);
				if (!openBraceIndent.IndentedTokenIsFirstAtLine)
					continue;
				if (openBraceIndent.LengthInSpaces != closeBraceIndent.LengthInSpaces)
				{
					yield return Report(braces.Open, $"Парные фигурные скобки ({braces}) должны иметь одинаковый отступ.");
				}
			}
		}

		private IEnumerable<string> ReportIfBracesContentNotIndentedOrNotConsistent()
		{
			foreach (var braces in bracesPairs)
			{
				if (braces.Open.GetLine() == braces.Close.GetLine())
					continue;
				var firstTokensOfBracesContentNodes = braces.Open.Parent.ChildNodes()
					.Select(node => node.DescendantTokens().First())
					.Where(n => n.SpanStart > braces.Open.SpanStart)
					.ToList();
				var openbraceIndent = new Indent(braces.Open);
				Indent bracesContentIndent = null;
				for (var i = 0; i < firstTokensOfBracesContentNodes.Count; i++)
				{
					var curIndent = new Indent(firstTokensOfBracesContentNodes[i]);
					if (!curIndent.IndentedTokenIsFirstAtLine && i > 0)
						continue;
					if (bracesContentIndent == null)
					{
						if (curIndent.LengthInSpaces > openbraceIndent.LengthInSpaces)
							bracesContentIndent = curIndent;
						else
						{
							yield return Report(firstTokensOfBracesContentNodes[i],
								$"Содержимое парных фигурных скобок ({braces}) должно иметь дополнительный отступ.");
						}
					}
					else if (curIndent.LengthInSpaces != bracesContentIndent.LengthInSpaces)
						yield return
							Report(firstTokensOfBracesContentNodes[i],
								$"Содержимое парных фигурных скобок ({braces}) должно иметь одинаковый отступ."); 
					// todo неправда для вложенных уровней. можно сделать два сообщения - для вложенных слабое правило - не меньше отступа первого уровня
				}
			}
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

		public override string ToString()
		{
			return $"{Open.GetLine() + 1}:{Close.GetLine() + 1}";
		}
	}

	internal class Indent
	{
		private SyntaxToken indentedToken;

		public int LengthInSpaces { get; }
		public bool IndentedTokenIsFirstAtLine { get; }

		public Indent(SyntaxToken indentedToken)
		{
			this.indentedToken = indentedToken;
			var tokenLineSpanStart = indentedToken.SyntaxTree.GetText().Lines[indentedToken.GetLine()].Start;
			var firstTokenInLine = indentedToken.SyntaxTree.GetRoot().FindToken(tokenLineSpanStart);
			IndentedTokenIsFirstAtLine = firstTokenInLine.IsEquivalentTo(indentedToken);
			LengthInSpaces = GetLengthInSpaces(firstTokenInLine);
		}

		private int GetLengthInSpaces(SyntaxToken firstTokenInLine)
		{
			var lastTrivia = firstTokenInLine.LeadingTrivia.LastOrDefault();

			var a = 0; /*fdfd*/ if (!lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia)) // todo edge case with comment
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

		public override string ToString()
		{
			return $"{indentedToken.Kind()}. LengthInSpaces: {LengthInSpaces}";
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