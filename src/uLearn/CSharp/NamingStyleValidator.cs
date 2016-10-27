using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class NamingStyleValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethod);
		}

		private IEnumerable<string> InspectMethod(MethodDeclarationSyntax method)
		{
			var name = method?.Identifier.Text;
			if (name == null)
				yield break;
			if (method.IsVoidGetter())
				yield return Report(method.Identifier, "'Get' метод без возвращаемого значения — это бессмыслица");
			if (method.IsNoArgsSetter())
				yield return Report(method.Identifier, "'Set' метод без аргументов — это бессмыслица");
			if (name.IsSingleWordGerundIdentifier())
				yield return Report(method.Identifier, "Называйте методы простыми глаголами! Например, Move, а не Moving");
			if (name.IsSingleWordIonIdentifier())
				yield return Report(method.Identifier, "Называйте методы глаголами! Например, Convert, а не Conversion");
		}
	}
}