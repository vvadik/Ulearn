using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using uLearn.CSharp;

namespace uLearn
{
	public class IndentsValidator : ICSharpSolutionValidator
	{
		private string errors;
		private SyntaxTree tree;
		private IEnumerable<SyntaxToken> openbraces;
		private IEnumerable<SyntaxToken> closebraces;

		public string FindError(SyntaxTree userSolution)
		{
			tree = userSolution;
			errors = null;
			openbraces = tree.GetRoot().DescendantTokens().Where(t => t.IsKind(SyntaxKind.OpenBraceToken));
			closebraces = tree.GetRoot().DescendantTokens().Where(t => t.IsKind(SyntaxKind.CloseBraceToken));

			ReportIfCompilationUnitChildrenIndented();
			ReportIfOpenBraceHasContentOnSameLine();
			ReportIfBracesContentNotIndented();
			ReportIfBracesNotAligned();
			return errors;
		}

		private void ReportIfCompilationUnitChildrenIndented()
		{
			foreach (var childNode in tree.GetRoot().ChildNodes())
				if (Indent.Exists(childNode))
					AddError($"Ќа строке {GetStartLinePosition(childNode).Line + 1} не должно быть отступа");
		}

		private void ReportIfBracesContentNotIndented()
		{
			foreach (var openbrace in openbraces)
			{
				if (closebraces.Any(b => TokensOnSameLine(b, openbrace)))
					continue;
				var children = openbrace.Parent.ChildNodesAndTokens();
				var bracesContentStartInd = children.FindIndex(openbrace) + 1;
				var bracesContentEndInd = children.FindIndex(c => c.IsKind(SyntaxKind.CloseBraceToken));
				var openbraceIndent = new Indent(openbrace, tree);
				Indent contentIndent = null;
				for (var i = bracesContentStartInd; i < bracesContentEndInd; i++)
				{
					var curIndent = new Indent(children[i], tree);
					if (curIndent.NodeOrTokenIsNotFirstAtLine())
						continue;
					if (contentIndent == null && curIndent.Length <= openbraceIndent.Length ||
						contentIndent != null && curIndent.Length != contentIndent.Length)
					{
						var units = openbraceIndent.Type == IndentType.Tab ? "табов" : "пробелов";
						AddError(
							$"Ќа строке {curIndent.Line + 1} в позиции {curIndent.Character} должен быть отступ размером в {openbraceIndent.Length} {units}");
					}
					else
						contentIndent = curIndent;
				}
			}
		}

		private void ReportIfOpenBraceHasContentOnSameLine()
		{
			foreach (var openbrace in openbraces)
			{
				if (closebraces.Any(b => TokensOnSameLine(b, openbrace)))
					continue;
				var endSpanOfOpenbrace = openbrace.HasTrailingTrivia
					? openbrace.TrailingTrivia.Last().Span.End
					: openbrace.Span.End;
				var firstTokenAfterOpenbrace = tree.GetRoot().FindToken(endSpanOfOpenbrace + 1);
				if (TokensOnSameLine(firstTokenAfterOpenbrace, openbrace))
				{
					AddError($"Ќа строке {GetLine(openbrace) + 1} после фигурной скобки должен быть отступ");
				}
			}
		}

		private bool TokensOnSameLine(SyntaxToken a, SyntaxToken b)
		{
			return GetLine(a) == GetLine(b);
		}

		private void ReportIfBracesNotAligned()
		{
			if (openbraces.Count() != closebraces.Count())
				return;
			var openbracesStack = new Stack<SyntaxToken>();
			foreach (var brace in openbraces.Concat(closebraces).OrderBy(b => GetLine(b)).ThenBy(t => GetCharacter(t)))
			{
				if (brace.IsKind(SyntaxKind.OpenBraceToken))
				{
					openbracesStack.Push(brace);
					continue;
				}
				var openBrace = openbracesStack.Pop();
				var openBraceIndent = new Indent(openBrace, tree);
				var closeBraceIndent = new Indent(brace, tree);
				if (openBraceIndent.NodeOrTokenIsNotFirstAtLine() || closeBraceIndent.NodeOrTokenIsNotFirstAtLine())
					continue;
				if (openBraceIndent.Length != closeBraceIndent.Length)
				{
					var units = openBraceIndent.Type == IndentType.Tab ? "табах" : "пробелах";
					AddError(
						$"–азмер отступа {closeBraceIndent.Length} (в {units}) на строке {GetLine(brace) + 1} " +
						$"должен совпадать с размером отступа {openBraceIndent.Length} на строке {GetLine(openBrace)}");
				}
			}
		}

		private static LinePosition GetStartLinePosition(SyntaxNodeOrToken nodeOrToken)
		{
			return GetStartLinePosition(nodeOrToken.GetLocation());
		}

		private static int GetLine(SyntaxNodeOrToken nodeOrToken)
		{
			return GetStartLinePosition(nodeOrToken.GetLocation()).Line;
		}

		private static int GetCharacter(SyntaxNodeOrToken nodeOrToken)
		{
			return GetStartLinePosition(nodeOrToken.GetLocation()).Character;
		}

		private static LinePosition GetStartLinePosition(Location location)
		{
			return location.GetLineSpan().StartLinePosition;
		}

		private void AddError(string msg)
		{
			errors += "ќшибка отступов! " + msg + "\r\n";
		}
	}

	internal class Indent
	{
		public string LeadingTrivia { get; } = String.Empty;
		public int Length => LeadingTrivia.Length;
		public IndentType Type { get; } = IndentType.Tab;
		public int Line { get; }
		public int Character { get; }
		public SyntaxNodeOrToken NodeOrToken { get; }

		public Indent(SyntaxNodeOrToken nodeOrToken, SyntaxTree tree)
		{
			NodeOrToken = nodeOrToken;
			Line = NodeOrToken.GetLocation().GetLineSpan().StartLinePosition.Line;
			Character = NodeOrToken.GetLocation().GetLineSpan().StartLinePosition.Character;
			var firstTokenInLine = tree.GetRoot().FindToken(tree.GetText().Lines[Line].Start);
			var lastTrivia = firstTokenInLine.LeadingTrivia.LastOrDefault();
			if (lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
			{
				LeadingTrivia = lastTrivia.ToFullString();
				Type = GetIndentType(LeadingTrivia);
			}
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

		public bool NodeOrTokenIsNotFirstAtLine()
		{
			return Length == 0 && Character > 0;
		}

		public override string ToString()
		{
			return $"{NodeOrToken.Kind()}. Length: {Length}";
		}
	}

	internal enum IndentType
	{
		Tab,
		Whitespace,
		Mixed
	}
}