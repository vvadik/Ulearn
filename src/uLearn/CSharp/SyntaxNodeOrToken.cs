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
        public bool IsKeyword { get; private set; }

        public static SyntaxNodeOrToken Create(SyntaxTree rootTree, SyntaxNode syntaxNode, bool isKeyword = false)
        {
            return new SyntaxNodeOrToken(rootTree) { SyntaxNode = syntaxNode, IsKeyword = isKeyword };
        }

        public static SyntaxNodeOrToken Create(SyntaxTree rootTree, SyntaxToken syntaxToken, bool isKeyword = false)
        {
            return new SyntaxNodeOrToken(rootTree) { SyntaxToken = syntaxToken, IsKeyword = isKeyword };
        }

        public bool IsEmpty()
        {
            return SyntaxNode == null && SyntaxToken == default(SyntaxToken);
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

        public int GetLine()
        {
            Location location = null;
            if (SyntaxNode != null)
                location = SyntaxNode.GetLocation();
            else if (SyntaxToken != default(SyntaxToken))
                location = SyntaxToken.GetLocation();

            if (location == null)
                return -1;

            var fileLinePositionSpan = location.GetLineSpan();
            return fileLinePositionSpan.StartLinePosition.Line + 1;
        }

        public int GetStartIndexInSpaces()
        {
            var sourceText = RootTree.GetText();
            var syntaxTrivias = GetSyntaxTrivias();
            var textSpan = syntaxTrivias.Count == 1
                ? syntaxTrivias.FullSpan
                : syntaxTrivias.LastOrDefault().FullSpan;
            var subText = sourceText.GetSubText(textSpan).ToString();
            return GetRealLength(subText);
        }

        private SyntaxTriviaList GetSyntaxTrivias()
        {
            if (SyntaxNode != null)
                return SyntaxNode.GetLeadingTrivia();
            if (SyntaxToken != default(SyntaxToken))
                return SyntaxToken.LeadingTrivia;
            return default(SyntaxTriviaList);
        }

        public bool HasExcessNewLines()
        {
            var syntaxTrivias = GetSyntaxTrivias();
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

        public IEnumerable<SyntaxNodeOrToken> GetStatementsSyntax()
        {
            if (SyntaxNode == null)
                yield break;
            switch (SyntaxNode)
            {
                case DoStatementSyntax doStatementSyntax:
                    yield return Create(RootTree, doStatementSyntax.Statement);
                    yield return Create(RootTree, doStatementSyntax.WhileKeyword, true);
                    break;
                case ElseClauseSyntax elseClauseSyntax:
                    yield return Create(RootTree, elseClauseSyntax.Statement);
                    break;
                case IfStatementSyntax ifStatementSyntax:
                    yield return Create(RootTree, ifStatementSyntax.Statement);
                    if (ifStatementSyntax.Else != null)
                        yield return Create(RootTree, ifStatementSyntax.Else, true);
                    break;
                case ForStatementSyntax forStatement:
                    var innerForStatement = forStatement.Statement;
                    yield return Create(RootTree, forStatement.Statement, innerForStatement is ForStatementSyntax);
                    break;
                case ForEachStatementSyntax foreachStatement:
                    var innerForeachStatement = foreachStatement.Statement;
                    yield return Create(RootTree, foreachStatement.Statement,
                        innerForeachStatement is ForEachStatementSyntax);
                    break;
                case WhileStatementSyntax whileStatement:
                    var innerWhileStatement = whileStatement.Statement;
                    yield return Create(RootTree, whileStatement.Statement,
                        innerWhileStatement is WhileStatementSyntax);
                    break;
                default:
                    yield break;
            }
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