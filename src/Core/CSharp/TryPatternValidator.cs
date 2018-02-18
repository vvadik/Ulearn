using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class TryPatternValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return userSolution.GetRoot()
				.DescendantNodes()
				.Where(node => node.Kind() == SyntaxKind.MethodDeclaration)
				.Cast<MethodDeclarationSyntax>()
				.Where(MethodHasOutParameter)
				.SelectMany(method => ValidateTryPattern(method).Select(validation => Report(method, validation)));
		}

		private static IEnumerable<string> ValidateTryPattern(MethodDeclarationSyntax methodDeclaration)
		{
			var arguments = methodDeclaration.ParameterList.Parameters.ToArray();
			var methodName = methodDeclaration.Identifier.ToString();
			if (!methodName.StartsWith("try", StringComparison.InvariantCultureIgnoreCase))
				yield return "Необходимо, чтобы название метода начиналось с Try";
			if (methodDeclaration.ReturnType.ToString() != "bool")
				yield return "Необходимо, чтобы метод возвращал bool";
			if (arguments.Count(ArgumentIsOutOrRef) > 1)
				yield return "В методе допустим только один ref/out параметр";
			if (!ArgumentIsOutOrRef(arguments.Last()))
				yield return "В методе ref/out параметр должен быть последним";
		}

		private static bool ArgumentIsOutOrRef(ParameterSyntax parameterSyntax)
		{
			return parameterSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword) || x.IsKind(SyntaxKind.RefKeyword));
		}

		private static bool MethodHasOutParameter(MethodDeclarationSyntax methodDeclaration)
		{
			var arguments = methodDeclaration.ParameterList.Parameters.ToArray();
			if (arguments.Length == 0)
				return false;
			return arguments.Any(ArgumentIsOutOrRef);
		}
	}
}