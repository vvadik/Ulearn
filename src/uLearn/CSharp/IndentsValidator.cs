using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using uLearn.CSharp;
using SyntaxNodeOrToken = uLearn.CSharp.SyntaxNodeOrToken;

namespace uLearn
{
    public class IndentsValidator : BaseStyleValidator
    {
        private const string prefix = "Код плохо отформатирован.\n" +
                                      "Автоматически отформатировать код в Visual Studio можно с помощью комбинации клавиш Ctrl+K+D";

        private SyntaxTree tree;
        private BracesPair[] bracesPairs;

        protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
        {
            tree = userSolution;
            bracesPairs = BuildBracesPairs().OrderBy(p => p.Open.SpanStart).ToArray();

            var errors = ReportIfCompilationUnitChildrenNotConsistent()
                .Concat(ReportIfBracesNotAligned())
                .Concat(ReportIfCloseBraceHasCodeOnSameLine())
                .Concat(ReportIfOpenBraceHasCodeOnSameLine())
                .Concat(ReportIfBracesContentNotIndentedOrNotConsistent())
                .Concat(ReportIfBracesNotIndented())
                .Concat(ReportIfNonBracesTokensHaveIncorrectIndents())
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

        private IEnumerable<string> ReportIfCompilationUnitChildrenNotConsistent()
        {
            var childLineIndents = tree.GetRoot().ChildNodes()
                .Select(node => node.GetFirstToken())
                .Select(t => new Indent(t))
                .Where(i => i.IndentedTokenIsFirstAtLine)
                .ToList();
            if (!childLineIndents.Any())
            {
                return Enumerable.Empty<string>();
            }
            var firstIndent = childLineIndents.First();
            return childLineIndents
                .Skip(1)
                .Where(i => i.LengthInSpaces != firstIndent.LengthInSpaces)
                .Select(i => Report(i.IndentedToken, "На верхнем уровне вложенности все узлы должны иметь одинаковый отступ"));
        }

        private IEnumerable<string> ReportIfBracesNotAligned()
        {
            foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
            {
                var openBraceIndent = new Indent(braces.Open);
                var closeBraceIndent = new Indent(braces.Close);
                if (openBraceIndent.IndentedTokenIsFirstAtLine && openBraceIndent.LengthInSpaces != closeBraceIndent.LengthInSpaces)
                {
                    yield return Report(braces.Open, $"Парные фигурные скобки ({braces}) должны иметь одинаковый отступ.");
                }
            }
        }

        private IEnumerable<string> ReportIfCloseBraceHasCodeOnSameLine()
        {
            foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
            {
                var openBraceIndent = new Indent(braces.Open);
                var closeBraceIndent = new Indent(braces.Close);
                if (openBraceIndent.IndentedTokenIsFirstAtLine && !closeBraceIndent.IndentedTokenIsFirstAtLine)
                {
                    yield return Report(braces.Close, "Перед закрывающей фигурной скобкой на той же строке не должно быть кода.");
                }
            }
        }

        private IEnumerable<string> ReportIfOpenBraceHasCodeOnSameLine()
        {
            foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
            {
                var openBraceHasCodeOnSameLine = braces.Open.Parent.ChildNodes()
                    .Select(node => node.GetFirstToken())
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
                var firstTokenOfLineWithMinimalIndent = Indent.TokenIsFirstAtLine(braces.Open)
                    ? braces.Open
                    : GetFirstTokenOfCorrectOpenbraceParent(braces.Open);
                if (firstTokenOfLineWithMinimalIndent == default(SyntaxToken))
                    continue;
                var minimalIndentAfterOpenbrace = new Indent(firstTokenOfLineWithMinimalIndent);
                var firstChild = childLineIndents.First();
                if (firstChild.LengthInSpaces <= minimalIndentAfterOpenbrace.LengthInSpaces)
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

        private IEnumerable<string> ReportIfNonBracesTokensHaveIncorrectIndents()
        {
            return tree.GetRoot().DescendantNodes()
                .Where(NeedToValidateNonBracesTokens)
                .Select(x => SyntaxNodeOrToken.Create(tree, x, true))
                .SelectMany(CheckNonBracesStatements)
                .Distinct();
        }

        private static IEnumerable<string> CheckNonBracesStatements(SyntaxNodeOrToken rootStatementSyntax)
        {
            var rootLine = rootStatementSyntax.GetStartLine();
            var rootEndLine = rootStatementSyntax.GetConditionEndLine();
            var parent = rootStatementSyntax.GetParent();
            var rootStart = rootStatementSyntax.GetValidationStartIndexInSpaces();

            if (parent != null && parent.Kind == SyntaxKind.Block)
            {
                var parentLine = parent.GetStartLine();
                if (parentLine == rootLine)
                    yield break;
                var parentStart = parent.GetValidationStartIndexInSpaces();
                if (rootStart == 0)
                    yield break;
                var validateIndent =
                    ValidateIndent(rootStatementSyntax, rootStart, parentStart, rootLine, parentLine);
                if (validateIndent != null)
                {
                    yield return Report(rootStatementSyntax, validateIndent);
                    yield break;
                }
            }

            var statementClauses = rootStatementSyntax.GetStatementsSyntax().ToArray();

            foreach (var statementClause in statementClauses)
            {
                if (statementClause.Kind == SyntaxKind.Block)
                    break;
                var statementStart = statementClause.GetValidationStartIndexInSpaces();
                var statementLine = statementClause.GetStartLine();

                if (statementClause.HasExcessNewLines())
                {
                    yield return Report(statementClause, "Выражение не должно иметь лишние " +
                                                         $"переносы строк после родителя ({GetNodePosition(rootStatementSyntax)}).");
                    continue;
                }
                if (!statementClause.OnSameIndentWithParent.HasValue)
                {
                    if (statementStart != rootStart)
                    {
                        var report = ValidateIndent(rootStatementSyntax, statementStart, rootStart, statementLine, rootLine);
                        if (report != null)
                        {
                            yield return Report(statementClause, report);
                            continue;
                        }
                    }
                }
                else
                {
                    if (statementClause.OnSameIndentWithParent.Value)
                    {
                        if (statementStart != rootStart)
                        {
                            yield return Report(statementClause, "Выражение должно иметь такой же отступ, " +
                                                                 $"как у родителя ({GetNodePosition(rootStatementSyntax)}).");
                            continue;
                        }
                    }
                    else
                    {
                        if (rootEndLine == statementLine
                            && (rootStatementSyntax.Kind == SyntaxKind.IfStatement ||
                                rootStatementSyntax.Kind == SyntaxKind.ElseClause)
                            && (statementClause.Kind != SyntaxKind.IfStatement
                                && statementClause.Kind != SyntaxKind.ForStatement
                                && statementClause.Kind != SyntaxKind.ForEachStatement
                                && statementClause.Kind != SyntaxKind.WhileStatement
                                && statementClause.Kind != SyntaxKind.DoStatement))
                            continue;
                        var report = ValidateIndent(rootStatementSyntax, statementStart, rootStart, statementLine, rootLine);
                        if (report != null)
                        {
                            yield return Report(statementClause, report);
                            continue;
                        }
                    }
                }
                foreach (var nestedError in CheckNonBracesStatements(statementClause))
                    yield return nestedError;
            }
        }

        private static string ValidateIndent(
            SyntaxNodeOrToken root,
            int statementStart,
            int rootStart,
            int statementLine,
            int rootLine)
        {
            if (statementLine == rootLine)
            {
                return "Выражение должно иметь дополнительный перенос строки " +
                       $"после родителя ({GetNodePosition(root)}).";
            }
            if (statementStart <= rootStart)
            {
                return "Выражение должно иметь отступ больше, " +
                       $"чем у родителя ({GetNodePosition(root)}).";
            }
            var delta = statementStart - rootStart;
            if (delta < 4)
            {
                return "Выражение должно иметь отступ, не меньше 4 пробелов " +
                       $"относительно родителя ({GetNodePosition(root)}).";
            }
            return null;
        }

        private static bool NeedToValidateNonBracesTokens(SyntaxNode syntaxNode)
        {
            var syntaxKind = syntaxNode.Kind();
            return syntaxKind == SyntaxKind.IfStatement
                   || syntaxKind == SyntaxKind.WhileStatement
                   || syntaxKind == SyntaxKind.ForStatement
                   || syntaxKind == SyntaxKind.ForEachStatement
                   || syntaxKind == SyntaxKind.DoStatement;
        }

        private static string GetNodePosition(SyntaxNodeOrToken nodeOrToken)
        {
            var linePosition = nodeOrToken.GetFileLinePositionSpan().StartLinePosition;
            return $"cтрока {linePosition.Line + 1}, позиция {linePosition.Character}";
        }

        private SyntaxToken GetFirstTokenOfCorrectOpenbraceParent(SyntaxToken openbrace)
        {
            if (openbrace.Parent is BaseTypeDeclarationSyntax ||
                openbrace.Parent is AccessorListSyntax ||
                openbrace.Parent is SwitchStatementSyntax ||
                openbrace.Parent is AnonymousObjectCreationExpressionSyntax ||
                openbrace.Parent.IsKind(SyntaxKind.ComplexElementInitializerExpression))
            {
                return openbrace.Parent.GetFirstToken();
            }
            if (openbrace.Parent is InitializerExpressionSyntax ||
                openbrace.Parent is BlockSyntax && !(openbrace.Parent.Parent is BlockSyntax))
            {
                return openbrace.Parent.Parent.GetFirstToken();
            }
            return default(SyntaxToken);
        }

        private IEnumerable<string> ReportIfBracesNotIndented()
        {
            foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine() &&
                                                             Indent.TokenIsFirstAtLine(pair.Open)))
            {
                var correctOpenbraceParent = GetFirstTokenOfCorrectOpenbraceParent(braces.Open);
                if (correctOpenbraceParent == default(SyntaxToken))
                    continue;
                var parentLineIndent = new Indent(correctOpenbraceParent);
                var openbraceLineIndent = new Indent(braces.Open);
                if (openbraceLineIndent.LengthInSpaces < parentLineIndent.LengthInSpaces)
                    yield return Report(braces.Open,
                        $"Парные фигурные скобки ({braces}) должны иметь отступ не меньше, чем у родителя.");
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