using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public class CSharpSolutionValidator : ISolutionValidator
	{
		private readonly List<ICSharpSolutionValidator> validators = new List<ICSharpSolutionValidator>
		{
			new NotEmptyCodeValidator(),
			new BlockLengthStyleValidator(),
			new LineLengthStyleValidator(),
			new NamingCaseStyleValidator(),
			new RedundantIfStyleValidator(),
			new NamingStyleValidator(),
		};

		public void AddValidator(ICSharpSolutionValidator validator)
		{
			validators.RemoveAll(item => item.GetType() == validator.GetType());
			validators.Add(validator);
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
			return error?.ToString();
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
}