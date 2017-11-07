using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
            var textSpan = default(TextSpan);
            if (SyntaxNode != null)
            {
                var syntaxTriviaList = SyntaxNode.GetLeadingTrivia();
                textSpan = syntaxTriviaList.Count == 1
                    ? syntaxTriviaList.FullSpan
                    : syntaxTriviaList.LastOrDefault().FullSpan;
            }
            if (SyntaxToken != default(SyntaxToken))
                textSpan = SyntaxToken.GetAllTrivia().First().FullSpan;
            var subText = sourceText.GetSubText(textSpan).ToString().Replace("\t", "    ");
            return subText.Length;
        }

        public IEnumerable<SyntaxNodeOrToken> GetStatementsSyntax()
        {
            if (SyntaxNode == null)
                yield break;
            switch (SyntaxNode)
            {
                case SwitchSectionSyntax switchSectionSyntax:
                    foreach (var switchSectionSyntaxStatement in switchSectionSyntax.Statements)
                        yield return Create(RootTree, switchSectionSyntaxStatement);
                    break;
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
                    yield return Create(RootTree, foreachStatement.Statement, innerForeachStatement is ForEachStatementSyntax);
                    break;
                case WhileStatementSyntax whileStatement:
                    var innerWhileStatement = whileStatement.Statement;
                    yield return Create(RootTree, whileStatement.Statement, innerWhileStatement is WhileStatementSyntax);
                    break;
                case SwitchStatementSyntax switchStatementSyntax:
                    foreach (var switchSectionSyntax in switchStatementSyntax.Sections)
                        yield return Create(RootTree, switchSectionSyntax);
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