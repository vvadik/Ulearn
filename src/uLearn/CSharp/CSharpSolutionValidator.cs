using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class CSharpSolutionValidator : ISolutionValidator
	{
		private static readonly ICSharpSolutionValidator redundantIf = new RedundantIfStyleValidator();
		private static readonly ICSharpSolutionValidator namingCase = new NamingCaseStyleValidator();
		private static readonly ICSharpSolutionValidator blockLength = new BlockLengthStyleValidator(25);
		private static readonly ICSharpSolutionValidator notEmpty = new HasStatementOrClassStyleValidator();

		private readonly List<ICSharpSolutionValidator> validators = new List<ICSharpSolutionValidator> { notEmpty, redundantIf, namingCase, blockLength };

		public CSharpSolutionValidator AddValidator(ICSharpSolutionValidator validator)
		{
			validators.RemoveAll(item => item.GetType() == validator.GetType());
			validators.Add(validator);
			return this;
		}

		public string FindFullSourceError(string userCode)
		{
			userCode = userCode.Trim();
			if (userCode.StartsWith("using") || userCode.StartsWith("namespace"))
				return "Не нужно писать весь исходный файл целиком — пишите только метод / класс, который необходим в задаче.";
			return null;
		}

		public string FindSyntaxError(string solution)
		{
			IEnumerable<Diagnostic> diagnostics = CSharpSyntaxTree.ParseText(solution).GetDiagnostics();
			var error = diagnostics.FirstOrDefault();
			return error != null ? error.ToString() : null;
		}

		public string FindValidatorError(string userCode, string solution)
		{
			try
			{
				SyntaxTree solutionTree = CSharpSyntaxTree.ParseText(userCode);
				return validators.Select(v => v.FindError(solutionTree)).FirstOrDefault(err => err != null);
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
	}

	internal class HasStatementOrClassStyleValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			var hasCode = userSolution.GetRoot().DescendantNodes().Any(n => n is StatementSyntax || n is MemberDeclarationSyntax);
			if (!hasCode)
				yield return Report(userSolution.GetRoot(), "Пустое решение?!");
		}
	}
}