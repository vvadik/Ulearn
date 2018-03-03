using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using uLearn.CSharp.IndentsValidation.Reporters;

namespace uLearn.CSharp.IndentsValidation
{
	public class IndentsValidator : BaseStyleValidator
	{
		private const string prefix = "Код плохо отформатирован.\n" +
									"Автоматически отформатировать код в Visual Studio можно с помощью комбинации клавиш Ctrl+K+D";

		private SyntaxTree tree;
		private BracesPair[] bracesPairs;

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			tree = userSolution;
			bracesPairs = BuildBracesPairs().OrderBy(p => p.Open.SpanStart).ToArray();

			var errors = CompilationUnitChildrenNotConsistentReporter.Report(tree)
				.Concat(BracesNotAlignedReporter.Report(bracesPairs))
				.Concat(CloseBraceHasCodeOnSameLineReporter.Report(bracesPairs))
				.Concat(OpenBraceHasCodeOnSameLineReporter.Report(bracesPairs))
				.Concat(BracesContentNotIndentedOrNotConsistentReporter.Report(bracesPairs))
				.Concat(BracesNotIndentedReporter.Report(bracesPairs))
				.Concat(NonBracesTokensHaveIncorrectIndentsReporter.Report(userSolution))
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
	}
}