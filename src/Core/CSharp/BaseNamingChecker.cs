using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public abstract class BaseNamingChecker : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<BaseTypeDeclarationSyntax>(userSolution, n => InspectName(n.Identifier))
				.Concat(InspectAll<TypeParameterSyntax>(userSolution, n => InspectName(n.Identifier)))
				.Concat(InspectAll<ParameterSyntax>(userSolution, n => InspectName(n.Identifier)))
				.Concat(InspectAll<EnumMemberDeclarationSyntax>(userSolution, n => InspectName(n.Identifier)))
				.Concat(InspectAll<MethodDeclarationSyntax>(userSolution, n => InspectName(n.Identifier)))
				.Concat(InspectAll<VariableDeclaratorSyntax>(userSolution, n => InspectName(n.Identifier)));
		}

		protected abstract IEnumerable<string> InspectName(SyntaxToken identifier);
	}
}