using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
    public class SyntaxNodeOrToken
    {
        public SyntaxTree RootTree { get; }

        private SyntaxNodeOrToken(SyntaxTree rootTree)
        {
            RootTree = rootTree;
        }

        public SyntaxNode SyntaxNode { get; set; }
        public SyntaxToken SyntaxToken { get; set; }
        public bool? OnSameIndentWithParent { get; private set; }

        public static SyntaxNodeOrToken Create(SyntaxTree rootTree, SyntaxNode syntaxNode,
            bool? onSameIndentWithParent = false)
        {
            return new SyntaxNodeOrToken(rootTree)
            {
                SyntaxNode = syntaxNode,
                OnSameIndentWithParent = onSameIndentWithParent
            };
        }

        public static SyntaxNodeOrToken Create(SyntaxTree rootTree, SyntaxToken syntaxToken,
            bool? onSameIndentWithParent = false)
        {
            return new SyntaxNodeOrToken(rootTree)
            {
                SyntaxToken = syntaxToken,
                OnSameIndentWithParent = onSameIndentWithParent
            };
        }

        public SyntaxKind Kind
        {
            get
            {
                if (SyntaxToken != default(SyntaxToken))
                    return SyntaxToken.Kind();
                if (SyntaxNode != null)
                    return SyntaxNode.Kind();
                return SyntaxKind.None;
            }
        }

        public int GetConditionEndLine()
        {
            var condition = GetCondition();
            if (condition == null)
                return GetStartLine();
            return condition.GetEndLine();
        }

        public int GetEndLine()
        {
            var linePositionSpan = GetFileLinePositionSpan();
            if (linePositionSpan.Equals(default(FileLinePositionSpan)))
                return -1;
            return linePositionSpan.EndLinePosition.Line + 1;
        }

        public int GetStartLine()
        {
            var linePositionSpan = GetFileLinePositionSpan();
            if (linePositionSpan.Equals(default(FileLinePositionSpan)))
                return -1;
            return linePositionSpan.StartLinePosition.Line + 1;
        }

        public FileLinePositionSpan GetFileLinePositionSpan()
        {
            Location location = null;
            if (SyntaxNode != null)
                location = SyntaxNode.GetLocation();
            else if (SyntaxToken != default(SyntaxToken))
                location = SyntaxToken.GetLocation();

            if (location == null)
                return default(FileLinePositionSpan);
            return location.GetLineSpan();
        }

        private static FileLinePositionSpan GetFileLinePositionSpan(SyntaxNode syntaxNode)
        {
            var location = syntaxNode.GetLocation();
            return location.GetLineSpan();
        }

        public int GetValidationStartIndexInSpaces()
        {
            var currentLine = GetStartLine();
            var parent = GetParent();
            var parentLine = parent.GetStartLine();
            if (parent.Kind == SyntaxKind.ElseClause
                && (Kind == SyntaxKind.IfStatement
                    || Kind == SyntaxKind.ExpressionStatement)
                && currentLine == parentLine)
                return parent.GetValidationStartIndexInSpaces();
            var sourceText = RootTree.GetText();
            var syntaxTrivias = GetLeadingSyntaxTrivias();
            var textSpan = syntaxTrivias.Count == 1
                ? syntaxTrivias.FullSpan
                : syntaxTrivias.LastOrDefault().FullSpan;
            var subText = sourceText.GetSubText(textSpan).ToString();
            return GetRealLength(subText);
        }

        public SyntaxNodeOrToken GetParent()
        {
            if (SyntaxNode != null)
                return Create(RootTree, SyntaxNode.Parent);
            if (SyntaxToken != default(SyntaxToken))
                return Create(RootTree, SyntaxToken.Parent);
            return null;
        }

        private SyntaxTriviaList GetLeadingSyntaxTrivias()
        {
            if (SyntaxNode != null)
                return SyntaxNode.GetLeadingTrivia();
            if (SyntaxToken != default(SyntaxToken))
                return SyntaxToken.LeadingTrivia;
            return default(SyntaxTriviaList);
        }

        public bool HasExcessNewLines()
        {
            var syntaxTrivias = GetLeadingSyntaxTrivias();
            return syntaxTrivias.Count > 1;
        }

        private int GetRealLength(string trivia)
        {
            var count = 0;
            var currentTabSpaces = 0;
            for (var i = 0; i < trivia.Length; ++i)
            {
                if (trivia[i] == '\t')
                {
                    if (currentTabSpaces == 0)
                        count += 4;
                    else
                        count += currentTabSpaces + (4 - currentTabSpaces);
                    currentTabSpaces = 0;
                }
                else if (trivia[i] == ' ')
                {
                    currentTabSpaces++;
                }
                if (currentTabSpaces == 4)
                {
                    count += 4;
                    currentTabSpaces = 0;
                }
            }
            return count + currentTabSpaces;
        }

        public SyntaxNodeOrToken GetCondition()
        {
            if (SyntaxNode == null)
                return null;
            switch (SyntaxNode)
            {
                case DoStatementSyntax doStatementSyntax:
                    return Create(RootTree, doStatementSyntax.Condition);
                case IfStatementSyntax ifStatementSyntax:
                    return Create(RootTree, ifStatementSyntax.Condition);
                case ForStatementSyntax forStatement:
                    return Create(RootTree, forStatement.Condition);
                case ForEachStatementSyntax foreachStatement:
                    return Create(RootTree, foreachStatement.Expression);
                case WhileStatementSyntax whileStatement:
                    return Create(RootTree, whileStatement.Condition);
            }
            return null;
        }

        public IEnumerable<SyntaxNodeOrToken> GetStatementsSyntax()
        {
            if (SyntaxNode == null)
                yield break;
            switch (SyntaxNode)
            {
                case DoStatementSyntax doStatementSyntax:
                    yield return Create(RootTree, doStatementSyntax.Statement);
                    yield return Create(RootTree, doStatementSyntax.WhileKeyword, true);
                    yield break;
                case ElseClauseSyntax elseClauseSyntax:
                    var elseClauseStatement = elseClauseSyntax.Statement;
                    yield return Create(RootTree, elseClauseStatement, elseClauseStatement is IfStatementSyntax
                                                                       && OnSameLine(elseClauseStatement,
                                                                           elseClauseSyntax));
                    yield break;
                case IfStatementSyntax ifStatementSyntax:
                    yield return Create(RootTree, ifStatementSyntax.Statement);
                    if (ifStatementSyntax.Else != null)
                        yield return Create(RootTree, ifStatementSyntax.Else, true);
                    yield break;
                case ForStatementSyntax forStatement:
                    var innerForStatement = forStatement.Statement;
                    yield return Create(RootTree, forStatement.Statement,
                        innerForStatement is ForStatementSyntax ? (bool?)null : false);
                    yield break;
                case ForEachStatementSyntax foreachStatement:
                    var innerForeachStatement = foreachStatement.Statement;
                    yield return Create(RootTree, foreachStatement.Statement,
                        innerForeachStatement is ForEachStatementSyntax ? (bool?)null : false);
                    yield break;
                case WhileStatementSyntax whileStatement:
                    var innerWhileStatement = whileStatement.Statement;
                    yield return Create(RootTree, whileStatement.Statement,
                        innerWhileStatement is WhileStatementSyntax ? (bool?)null : false);
                    yield break;
                default:
                    yield break;
            }
        }

        private bool OnSameLine(SyntaxNode first, SyntaxNode second)
        {
            var firstPositionSpan = GetFileLinePositionSpan(first);
            var secondPositionSpan = GetFileLinePositionSpan(second);
            return secondPositionSpan.StartLinePosition.Line == firstPositionSpan.StartLinePosition.Line;
        }

        public override string ToString()
        {
            if (SyntaxNode != null)
                return SyntaxNode.ToString();
            if (SyntaxToken != default(SyntaxToken))
                return SyntaxToken.ToString();
            return base.ToString();
        }
    }
}