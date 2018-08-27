using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp.Validators
{
	public class RedundantElseValidator: BaseStyleValidator
	{
		public override List<SolutionStyleError> FindErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<IfStatementSyntax>(userSolution, InspectIfStatement).ToList();
		}

		private IEnumerable<SolutionStyleError> InspectIfStatement(IfStatementSyntax ifStatementSyntax)
		{
			var childNodes = ifStatementSyntax
				.ChildNodes()
				.ToList();
			if (childNodes.Count <= 2)
				yield break;

			var correspondingElseClause = childNodes[2];
			var statementUnderIf = childNodes[1];
			
			switch (statementUnderIf)
			{
				case ReturnStatementSyntax _:
				case ThrowStatementSyntax _:
					yield return new SolutionStyleError(StyleErrorType.RedundantElse01, correspondingElseClause);
					break;
				case BlockSyntax blockSyntax:
					var statementsInBlock = blockSyntax.ChildNodes();
					var returnStatementsInBlock = statementsInBlock.OfType<ReturnStatementSyntax>();
					if (returnStatementsInBlock.Any())
						yield return new SolutionStyleError(StyleErrorType.RedundantElse01, correspondingElseClause);
					var throwStatementsInBlock = statementsInBlock.OfType<ThrowStatementSyntax>();
					if (throwStatementsInBlock.Any())
						yield return new SolutionStyleError(StyleErrorType.RedundantElse01, correspondingElseClause);
					break;
			}
		}
	}
}