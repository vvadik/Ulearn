using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RecursionStyleValidatorAttribute : Attribute, ICSharpSolutionValidator, IStrictValidator
	{
		private readonly bool requireRecursion;

		public RecursionStyleValidatorAttribute(bool requireRecursion)
		{
			this.requireRecursion = requireRecursion;
		}

		public string FindError(SyntaxTree userSolution)
		{
			var recursiveMethods = userSolution.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Where(IsRecursive).ToList();

			if (requireRecursion && !recursiveMethods.Any())
				return "Решение должно быть рекурсивным";

			if (!requireRecursion && recursiveMethods.Any())
				return "Решение должно быть нерекурсивным";

			return null;
		}

		private static bool IsRecursive(MethodDeclarationSyntax method)
		{
			return
				method.Body != null
				&& method.Body.DescendantNodes()
					.OfType<SimpleNameSyntax>()
					.Any(n => n.Identifier.ValueText == method.Identifier.ValueText);
		}
	}
}