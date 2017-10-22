using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class ExponentiationValidator: BaseStyleValidatorWithSemanticModel
	{
		private static readonly HashSet<string> forbiddenDegrees = 
			new HashSet<string>{"2", "3"};
		
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<InvocationExpressionSyntax>(userSolution, semanticModel, InspectMethod);
		}

		private IEnumerable<string> InspectMethod(InvocationExpressionSyntax methodInvocation, SemanticModel semanticModel)
		{
			var symbol = semanticModel.GetSymbolInfo(methodInvocation).Symbol;
			if (methodInvocation.IsMathPow(symbol) &&
				methodInvocation.ArgumentList.ContainsForbiddenDegrees(forbiddenDegrees))
				yield return Report(methodInvocation, "вызов Math.Pow для маленьких степеней неэффективен");
		}
	}
}