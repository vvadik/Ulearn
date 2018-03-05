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
			var arguments = methodDeclaration.ParameterList.Parameters.ToArray();
			if (arguments.Length == 0)
				return false;
			return arguments.Any(ArgumentIsRef);
		}

		private static bool ArgumentIsRef(ParameterSyntax parameterSyntax)
		{
			return parameterSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword));
		}

		private IEnumerable<string> ValidateRefs(
			MethodDeclarationSyntax methodDeclaration,
			SemanticModel semanticModel)
		{
			var parameters = methodDeclaration.ParameterList.Parameters.Where(ArgumentIsRef).ToArray();
			foreach (var parameter in parameters)
			{
				var info = semanticModel.GetTypeInfo(parameter.Type);
				if (!IsPrimitive(info.Type))
					yield return Report(methodDeclaration, $"Ошибка в аргументе: {parameter.Identifier}");
			}
		}

		private static bool IsPrimitive(ITypeSymbol typeSymbol)
		{
			switch (typeSymbol.SpecialType)
			{
				case SpecialType.System_Boolean:
				case SpecialType.System_Char:
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
					return true;
			}
			return false;
		}
	}
}