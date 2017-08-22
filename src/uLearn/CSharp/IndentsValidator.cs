using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using uLearn.CSharp;

namespace uLearn
{
	public class
		IndentsValidator : CSharpSyntaxWalker,
			ICSharpSolutionValidator
	{
		private string errors;
		private SyntaxTree tree;
		private List<Indent> orderedLineIndents;
		private IEnumerable<SyntaxToken> openbraces;
		private IEnumerable<SyntaxToken> closebraces;

		public string FindError(SyntaxTree userSolution)
		{
			tree = userSolution;
			errors = null;
			orderedLineIndents = BuildIndentsOfLines();
			openbraces = userSolution.GetRoot().DescendantTokens(t => t.IsKind(SyntaxKind.OpenBraceToken));
			closebraces = userSolution.GetRoot().DescendantTokens(t => t.IsKind(SyntaxKind.CloseBraceToken));

			ReportIfCompilationUnitChildrenIndented();
			ReportIfBracesContentNotIndented();
			ReportIfBracesNotAligned();
			return errors;
		}

		private List<Indent> BuildIndentsOfLines()
		{
			return tree.GetText().Lines
				.OrderBy(tl => tl.LineNumber)
				.Select(line => line.Span.IsEmpty
					? Indent.None
					: new Indent(tree.GetRoot().FindToken(line.Span.Start)))
				.ToList();
		}

		private void ReportIfCompilationUnitChildrenIndented()
		{
			foreach (var childNode in tree.GetRoot().ChildNodes())
				if (Indent.Exists(childNode))
					AddError($"Ќа строке {GetStartLinePosition(childNode).Line} не должно быть отступа");
		}

		private void ReportIfBracesContentNotIndented()
		{
			if (!orderedLineIndents.Any())
				return;
			for (var i = 1; i < orderedLineIndents.Count; i++)
			{
				if (orderedLineIndents[i].IsNone())
					continue;
				var curIndent = orderedLineIndents[i];
				var prevIndent = orderedLineIndents[i - 1];
				if (curIndent.NodeOrToken.IsKind(SyntaxKind.CloseBraceToken) ||
					!prevIndent.NodeOrToken.IsKind(SyntaxKind.OpenBraceToken))
					continue;
				if (curIndent.LessThanOrEqual(prevIndent) || curIndent.LessThan(prevIndent))
				{
					var units = curIndent.Type == IndentType.Tab ? "табов" : "пробелов";
					AddError(
						$"Ќа строке {GetLine(curIndent)} в позиции {GetCharacter(curIndent)} должен быть отступ размером в {prevIndent.Length} {units}");
				}
			}
		}

		private void ReportIfBracesNotAligned()
		{
			if (openbraces.Count() != closebraces.Count())
				return;
			var openbracesStack = new Stack<SyntaxToken>();
			foreach (var brace in openbraces.Concat(closebraces))
			{
				if (brace.IsKind(SyntaxKind.OpenBraceToken))
				{
					openbracesStack.Push(brace);
					continue;
				}
				var openBrace = openbracesStack.Pop();
				var openBraceIndent = new Indent(openBrace);
				var closeBraceIndent = new Indent(brace);
				if (openBraceIndent.IsNone() ||
					openBraceIndent.Type == IndentType.Mixed ||
					openBraceIndent.Type != closeBraceIndent.Type)
					return;
				if (openBraceIndent.Length != closeBraceIndent.Length)
				{
					var units = openBraceIndent.Type == IndentType.Tab ? "табах" : "пробелах";
					AddError(
						$"–азмер отступа {closeBraceIndent.Length} (в {units}) на строке {GetLine(brace)} " +
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

		private static int GetCharacter(Indent indent)
		{
			return GetStartLinePosition(indent.Location).Character;
		}

		private static int GetLine(Indent indent)
		{
			return GetStartLinePosition(indent.Location).Line;
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
		public static readonly Indent None = new Indent(default(SyntaxNodeOrToken));
		public string LeadingTrivia { get; } = String.Empty;
		public int Length => LeadingTrivia.Length;
		public IndentType Type { get; }
		public Location Location => NodeOrToken.GetLocation();
		public SyntaxNodeOrToken NodeOrToken { get; }

		public Indent(SyntaxNodeOrToken nodeOrToken)
		{
			NodeOrToken = nodeOrToken;
			var lastTrivia = NodeOrToken.GetLeadingTrivia().LastOrDefault();
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

		public bool IsNone()
		{
			return !LeadingTrivia.Any();
		}

		public static bool Exists(SyntaxNodeOrToken nodeOrToken)
		{
			return nodeOrToken.GetLeadingTrivia().LastOrDefault().IsKind(SyntaxKind.WhitespaceTrivia);
		}

		public bool LessThan(Indent i)
		{
			return Length < i.Length;
		}

		public bool LessThanOrEqual(Indent i)
		{
			return Length <= i.Length;
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