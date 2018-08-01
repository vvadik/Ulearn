using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp.Validators
{
	public class RedundantElseValidator: BaseStyleValidator
	{
		public override List<SolutionStyleError>  FindErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<IfStatementSyntax>(userSolution, InspectIfStatement).ToList();
		}

		private IEnumerable<SolutionStyleError> InspectIfStatement(IfStatementSyntax ifStatementSyntax)
		{
			var childNodes = ifStatementSyntax
				.ChildNodes()
				.ToList();
			if (!DoesIfStatementContainsElseClause(childNodes))
				yield break;

			var correspondindElseClause = childNodes[2];
			var statementUnderIf = childNodes[1];
			
			switch (statementUnderIf)
			{
				case ReturnStatementSyntax _:
					yield return new SolutionStyleError(StyleErrorType.RedundantElse01, correspondindElseClause);
					break;
				case BlockSyntax blockSyntax:
					var lastStatementInBlock = blockSyntax
						.ChildNodes()
						.LastOrDefault();
					if (lastStatementInBlock is ReturnStatementSyntax)
						yield return new SolutionStyleError(StyleErrorType.RedundantElse01, correspondindElseClause);
					break;
			}
		}

		private static bool DoesIfStatementContainsElseClause(List<SyntaxNode> childNodes)
		{
			return childNodes.Count > 2;
		}
	}
}