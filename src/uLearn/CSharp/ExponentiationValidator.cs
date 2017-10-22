using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class ExponentiationValidator: BaseStyleValidator
	{
		private SemanticModel semanticModel;
		private static readonly HashSet<string> forbiddenDegrees = 
			new HashSet<string>{"2", "3"};
		
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			var compilation = CSharpCompilation.Create("MyCompilation",
				syntaxTrees: new[] { userSolution }, references: new[] { mscorlib });
			semanticModel = compilation.GetSemanticModel(userSolution);
			return InspectAll<InvocationExpressionSyntax>(userSolution, InspectMethod);
		}

		private IEnumerable<string> InspectMethod(InvocationExpressionSyntax methodInvocation)
		{
			var symbol = semanticModel.GetSymbolInfo(methodInvocation).Symbol;
			if (methodInvocation.IsMathPow(symbol) &&
				methodInvocation.ArgumentList.ContainsForbiddenDegrees(forbiddenDegrees))
				yield return Report(methodInvocation, "вызов Math.Pow для маленьких степеней неэффективен");
		}
	}
}