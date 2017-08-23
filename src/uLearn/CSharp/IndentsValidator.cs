using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using uLearn.CSharp;

namespace uLearn
{
	public class IndentsValidator : ICSharpSolutionValidator // todo basevalidator
	{
		private string errors;
		private SyntaxTree tree;
		private IEnumerable<SyntaxToken> openbraces;
		private IEnumerable<SyntaxToken> closebraces;

		public string FindError(SyntaxTree userSolution) // todo FindeErrorInternal с префиксом
		{
			tree = userSolution;
			errors = null;
			openbraces = tree.GetRoot().DescendantTokens().Where(t => t.IsKind(SyntaxKind.OpenBraceToken));// todo составить пары
			closebraces = tree.GetRoot().DescendantTokens().Where(t => t.IsKind(SyntaxKind.CloseBraceToken));

			if (openbraces.Count() == closebraces.Count())
			{
				ReportIfCompilationUnitChildrenIndented();
				ReportIfBracesContentNotIndented();
			}
			ReportIfBracesNotAligned();
			return errors;
		}

		private void ReportIfCompilationUnitChildrenIndented()
		{
			foreach (var childNode in tree.GetRoot().ChildNodes())
				if (Indent.Exists(childNode))
					AddError($"Ќа верхнем уровне вложенности на строке {GetLine(childNode) + 1} не должно быть отступов.");
		}

		private void ReportIfBracesContentNotIndented()
		{
			foreach (var openbrace in openbraces)
			{
				// todo will fail with `new[] {1, 2, 3} new [] {1, \r\n 2, 3}`
				if (closebraces.Any(b => TokensOnSameLine(b, openbrace)))
					continue;
				var bracesContentNodes = openbrace.Parent.ChildNodes().Where(n => n.SpanStart > openbrace.SpanStart).ToList();
				var openbraceIndent = new Indent(openbrace, tree);
				Indent bracesContentIndent = null;
				for (var i = 0; i < bracesContentNodes.Count; i++)
				{
					var curIndent = new Indent(bracesContentNodes[i], tree);
					if (!curIndent.NodeOrTokenIsFirstInLine() && i > 0)
						continue;
					if (bracesContentIndent == null)
					{
						if (curIndent.Length > openbraceIndent.Length)
							bracesContentIndent = curIndent;
						else
						{
							AddError($"Ќа строке {curIndent.Line + 1} должен быть отступ. " +
									"≈сли открывающа€с€ фигурна€ скобка закрываетс€ на другой строке, то внутри фигурных скобок должен быть отступ.");
						}
					}
					else if (curIndent.Length != bracesContentIndent.Length)
						AddError(
							$"ќтступ на строке {curIndent.Line + 1} не консистентен. ¬нутри одних фигурных скобок отступ не должен измен€ть свой размер.");
				}
			}
		}

		private bool TokensOnSameLine(SyntaxToken a, SyntaxToken b)
		{
			return GetLine(a) == GetLine(b);
		}

		private void ReportIfBracesNotAligned()
		{
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
				if (!openBraceIndent.NodeOrTokenIsFirstInLine())
					continue;
				if (openBraceIndent.Length != closeBraceIndent.Length)
				{
					AddError(
						$"ќтступ перед фигурной скобкой на строке {GetLine(brace) + 1} должен быть таким же, " +
						$"как отступ перед фигурной скобкой на строке {GetLine(openBrace)}.");
				}
			}
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
		public readonly IndentType Type = IndentType.Tab;
		public readonly string LeadingTrivia = String.Empty;
		public readonly int Line;
		public readonly int Character;
		public int Length => LeadingTrivia.Length;
		public SyntaxNodeOrToken NodeOrToken { get; }
		private readonly SyntaxTree tree;

		public Indent(SyntaxNodeOrToken nodeOrToken, SyntaxTree tree)
		{
			NodeOrToken = nodeOrToken;
			this.tree = tree;
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

		public bool NodeOrTokenIsFirstInLine()
		{
			var firstTokenInLine = tree.GetRoot().FindToken(tree.GetText().Lines[Line].Start);
			var token = NodeOrToken.IsToken ? NodeOrToken.AsToken() : NodeOrToken.AsNode().DescendantTokens().First();
			return firstTokenInLine.IsEquivalentTo(token);
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