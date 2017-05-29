using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class RecursionStyleValidator : BaseStyleValidator
	{
		private readonly bool requireRecursion;

		public RecursionStyleValidator(bool requireRecursion)
		{
			this.requireRecursion = requireRecursion;
		}

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			var recursiveMethods = userSolution.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Where(IsRecursive).ToList();
			if (requireRecursion && !recursiveMethods.Any())
				yield return Report(userSolution.GetRoot(), "Решение должно быть рекурсивным");
			if (!requireRecursion && recursiveMethods.Any())
				yield return Report(userSolution.GetRoot(), "Решение должно быть нерекурсивным");
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