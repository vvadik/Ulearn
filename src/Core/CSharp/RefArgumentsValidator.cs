using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class RefArgumentsValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution,
			SemanticModel semanticModel)
		{
			var validationResult = userSolution.GetRoot()
				.DescendantNodes()
				.Where(node => node.Kind() == SyntaxKind.MethodDeclaration)
				.Cast<MethodDeclarationSyntax>()
				.Where(MethodHasRefParameter)
				.SelectMany(t => ValidateRefs(t, semanticModel))
				.ToArray();

			if (validationResult.Length == 0)
				yield break;
			yield return "Разрешено передавать через ref только примитивные типы";
			foreach (var error in validationResult)
				yield return error;
		}

		private static bool MethodHasRefParameter(MethodDeclarationSyntax methodDeclaration)
		{
			return methodDeclaration.ParameterList.Parameters.Any(ArgumentIsRef);
		}

		private static bool ArgumentIsRef(ParameterSyntax parameterSyntax)
		{
			return parameterSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword));
		}

		private IEnumerable<string> ValidateRefs(
			MethodDeclarationSyntax methodDeclaration,
			SemanticModel semanticModel)
		{
			string ReportLocal(ParameterSyntax parameter)
			{
				return Report(methodDeclaration, $"Ошибка в аргументе: {parameter.Identifier}");
			}

			return methodDeclaration.ParameterList.Parameters
				.Where(ArgumentIsRef)
				.Select(it => (Parameter: it, TypeInfo: semanticModel.GetTypeInfo(it.Type)))
				.Where(it => !IsPrimitive(it.TypeInfo.Type))
				.Select(it => ReportLocal(it.Parameter));
		}

		private static bool IsPrimitive(ITypeSymbol typeSymbol)
		{
			var specialTypeString = typeSymbol.SpecialType.ToString();
			if (!specialTypeString.StartsWith("System", StringComparison.InvariantCultureIgnoreCase))
				return false;
			return Type.GetType(specialTypeString.Replace('_', '.'))?.IsPrimitive == true;
		}
	}
}