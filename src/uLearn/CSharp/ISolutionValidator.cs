using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public interface ISolutionValidator
	{
		string FindError(SyntaxTree userSolution);
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class IsStaticMethodAttribute : Attribute, ISolutionValidator
	{
		public const string ShouldBeMethod = "Решение должно быть корректным определением статического метода";
		public const string ShouldBeSingleMethod = "Решение должно состоять ровно из одного метода";

		public string FindError(SyntaxTree userSolution)
		{
			var cu = userSolution.GetRoot() as CompilationUnitSyntax;
			if (cu == null) return ShouldBeMethod;
			if (cu.Members.Count > 1) return ShouldBeSingleMethod;
			var method = cu.Members[0] as MethodDeclarationSyntax;
			if (method == null) return ShouldBeMethod;
			return FindError(method);
		}

		protected virtual string FindError(MethodDeclarationSyntax method)
		{
			return null;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class SingleStatementMethodAttribute : IsStaticMethodAttribute
	{
		public const string ShouldBeSingleMethodMessage = "Решение должно быть в одно выражение";

		protected override string FindError(MethodDeclarationSyntax method)
		{
			return method.Body.Statements.Count != 1 ? ShouldBeSingleMethodMessage : null;
		}
	}
}