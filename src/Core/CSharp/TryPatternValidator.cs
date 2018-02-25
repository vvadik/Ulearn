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
			var validationResult = userSolution.GetRoot()
				.DescendantNodes()
				.Where(node => node.Kind() == SyntaxKind.MethodDeclaration)
				.Cast<MethodDeclarationSyntax>()
				.Where(MethodHasOutOrRefParameter)
				.SelectMany(method => ValidateTryPattern(method).Select(error => Report(method, error)))
				.ToArray();
			if (validationResult.Length == 0)
				yield break;
			yield return
				"Использование ref/out параметров в методах недопустимо. Существует лишь одно исключение: Try-паттерн. " +
				"Чтобы ваш код соблюдал все правила этого паттерна, вам надо исправить следующие ошибки:";
			foreach (var error in validationResult)
				yield return error;
		}

		private static IEnumerable<string> ValidateTryPattern(MethodDeclarationSyntax methodDeclaration)
		{
			var arguments = methodDeclaration.ParameterList.Parameters.ToArray();
			var methodName = methodDeclaration.Identifier.ToString();
			if (!methodName.StartsWith("try", StringComparison.InvariantCultureIgnoreCase))
				yield return "Название метода не начинается с Try";
			if (methodDeclaration.ReturnType.ToString() != "bool")
				yield return "Метод не возвращает bool";
			if (arguments.Count(ArgumentIsRef) > 0)
			{
				yield return "Недопустимо использовать ref параметры, разрешено использовать только 1 out параметр";
				yield break;
			}
			var outParametersCount = arguments.Count(ArgumentIsOut);
			if (outParametersCount > 1)
				yield return "Допустим только один out параметр в сигнатуре метода";
			if (outParametersCount == 1 && !ArgumentIsOut(arguments.Last()))
				yield return "Out параметр не является последним аргументом";
		}

		private static bool ArgumentIsOut(ParameterSyntax parameterSyntax)
		{
			return parameterSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword));
		}

		private static bool ArgumentIsRef(ParameterSyntax parameterSyntax)
		{
			return parameterSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword));
		}

		private static bool MethodHasOutOrRefParameter(MethodDeclarationSyntax methodDeclaration)
		{
			var arguments = methodDeclaration.ParameterList.Parameters.ToArray();
			if (arguments.Length == 0)
				return false;
			return arguments.Any(x => ArgumentIsOut(x) || ArgumentIsRef(x));
		}
	}
}