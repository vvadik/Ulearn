using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class VerbInMethodNameValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethodsNames);
		}

		private IEnumerable<string> InspectMethodsNames(MethodDeclarationSyntax methodDeclarationSyntax)
		{
			//var a = methodDeclarationSyntax.Identifier();
		}
	}
}