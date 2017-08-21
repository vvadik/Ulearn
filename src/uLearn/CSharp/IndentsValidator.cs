using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using uLearn.CSharp;

namespace uLearn
{
	public class
		IndentsValidator : CSharpSyntaxWalker,
			ICSharpSolutionValidator
	{
		private string errors;

		public string FindError(SyntaxTree userSolution)
		{
			errors = null;
			Visit(userSolution.GetRoot());
			return errors;
		}

		public override void Visit(SyntaxNode node)
		{
			ReportIfCompilationUnitChildrenIndented(node);
			ReportIfBraceTokensNotAligned(node);
			ReportIfBaseTypeChildNotIndented(node);
			base.Visit(node);
		}

		private void ReportIfBaseTypeChildNotIndented(SyntaxNode node)
		{
			if (!(node is BaseTypeDeclarationSyntax))
				return;
			var parentIndent = GetIndent(node);
			var openBraceInd = node.ChildNodesAndTokens().FindIndex(i => i.IsKind(SyntaxKind.OpenBraceToken));
			var closeBraceInd = node.ChildNodesAndTokens().FindIndex(i => i.IsKind(SyntaxKind.CloseBraceToken));
			for (var ind = openBraceInd + 1; ind < closeBraceInd; ind++)
			{
				var childNode = (SyntaxNode)node.ChildNodesAndTokens()[ind];
				if (childNode == null || !IsFirstChildOfParent(childNode, node))
					continue;
				var childIndent = GetIndent(childNode);
				if (childIndent.Type == IndentType.Mixed || parentIndent.Type != childIndent.Type)
					return;
				if (childIndent.Length <= parentIndent.Length)
				{
					var linePosition = GetNodeLinePosition(childNode);
					var units = childIndent.Type == IndentType.Tab ? "табов" : "пробелов";
					AddError(
						$"Ќа строке {linePosition.Line} в позиции {linePosition.Character} должен быть отступ размером в {parentIndent.Length} {units}");
				}
			}
		}

		private bool IsFirstChildOfParent(SyntaxNode child, SyntaxNode parent)
		{
			return parent.ChildNodes().FindIndex(c => c.Span == child.Span) == 0;
		}

		private Indent GetIndent(SyntaxNode node) // todo баг, когда несколько нод в одной строке
		{
			var currentNode = node;
			while (!currentNode.GetLeadingTrivia().Any())
			{
				currentNode = currentNode.Parent;
				if (currentNode == null)
					return Indent.None;
			}
			return GetIndent(currentNode.GetLeadingTrivia());
		}

		private Indent GetIndent(SyntaxTriviaList leadingTriviaList)
		{
			var lastTrivia = leadingTriviaList.LastOrDefault();
			if (lastTrivia == default(SyntaxTrivia) || !lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
				return Indent.None;
			return new Indent(lastTrivia);
		}

		private void ReportIfCompilationUnitChildrenIndented(SyntaxNode node)
		{
			var indent = GetIndent(node);
			if (node.Parent.IsKind(SyntaxKind.CompilationUnit) && !indent.IsEmpty())
			{
				AddError($"Ќа строке {GetNodeLinePosition(node).Line} не должно быть отступа");
			}
		}

		private static LinePosition GetNodeLinePosition(SyntaxNode node)
		{
			return node.GetLocation().GetLineSpan().StartLinePosition;
		}

		private static int GetTokenLine(SyntaxToken token)
		{
			return token.GetLocation().GetLineSpan().StartLinePosition.Line;
		}

		private void ReportIfBraceTokensNotAligned(SyntaxNode node)
		{
			var openBrace = node.ChildTokens().FirstOrDefault(token => token.IsKind(SyntaxKind.OpenBraceToken));
			var closeBrace = node.ChildTokens().FirstOrDefault(token => token.IsKind(SyntaxKind.CloseBraceToken));
			if (openBrace == default(SyntaxToken) || closeBrace == default(SyntaxToken))
				return;
			if (GetTokenLine(openBrace) == GetTokenLine(closeBrace))
				return;
			var openBraceIndent = GetIndent(openBrace.LeadingTrivia);
			var closeBraceIndent = GetIndent(closeBrace.LeadingTrivia);
			if (openBraceIndent.Type == IndentType.Mixed || openBraceIndent.Type != closeBraceIndent.Type)
				return;
			if (openBraceIndent.Length != closeBraceIndent.Length)
			{
				var units = openBraceIndent.Type == IndentType.Tab ? "табах" : "пробелах";
				AddError(
					$"–азмер отступа {closeBraceIndent.Length} (в {units}) на строке {GetTokenLine(closeBrace)} " +
					$"должен совпадать с размером отступа {openBraceIndent.Length} на строке {GetTokenLine(openBrace)}");
			}
		}

		private void AddError(string msg)
		{
			errors += "ќшибка отступов! " + msg + "\r\n";
		}
	}

	internal class Indent
	{
		public static readonly Indent None = new Indent();
		public string LeadingTrivia { get; } = String.Empty;
		public int Length => LeadingTrivia.Length;
		public IndentType Type { get; }
		public Location Location { get; }

		public Indent(SyntaxTrivia leadingTrivia)
		{
			LeadingTrivia = leadingTrivia.ToFullString();
			Location = leadingTrivia.GetLocation();
			Type = GetIndentType(LeadingTrivia);
		}

		private Indent()
		{
		}

		public static IndentType GetIndentType(string value)
		{
			if (value.All(v => v == '\t'))
				return IndentType.Tab;
			if (value.All(v => v == ' '))
				return IndentType.Whitespace;
			return IndentType.Mixed;
		}

		public bool IsEmpty()
		{
			return !LeadingTrivia.Any();
		}
	}

	internal enum IndentType
	{
		Tab,
		Whitespace,
		Mixed
	}
}