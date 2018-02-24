using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class ArrayLengthStyleValidator: BaseStyleValidatorWithSemanticModel
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			var errorsInForCycle = InspectAll<ForStatementSyntax>(userSolution, semanticModel, InspectMethod);
			var errorsInForEachCycle = InspectAll<ForEachStatementSyntax>(userSolution, semanticModel, InspectMethod);
			var errorsInWhileCycle = InspectAll<WhileStatementSyntax>(userSolution, semanticModel, InspectMethod);
			var errorsInDoWhileCycle = InspectAll<DoStatementSyntax>(userSolution, semanticModel, InspectMethod);

			return errorsInForCycle
				.Concat(errorsInForEachCycle)
				.Concat(errorsInWhileCycle)
				.Concat(errorsInDoWhileCycle);
		}

		private IEnumerable<string> InspectMethod<TCycle>(TCycle cycleStatement, SemanticModel semanticModel) where TCycle: StatementSyntax // TODO: вытаскивать все инициализации
		{
			var methodInvocations = cycleStatement.DescendantNodes().OfType<InvocationExpressionSyntax>();

			foreach (var methodInvocation in methodInvocations)
			{
				var symbol = ModelExtensions.GetSymbolInfo(semanticModel, methodInvocation).Symbol;
				var argumentExpression = methodInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
				if (symbol?.ToString() != "System.Array.GetLength(int)"
					|| argumentExpression == null
					|| !argumentExpression.IsKind(SyntaxKind.NumericLiteralExpression))
					continue;
				
				var variable = methodInvocation.Expression;
				if (!cycleStatement.ContainsAssignmentOf(variable.ToString()))
					yield return Report(methodInvocation, "Неэффективный код. GetLength вызывается в цикле для константы и возвращает каждый раз одно и то же. Лучше вынести результат выполнения метода в переменную за цикл.");
			}
		}
	}
}
