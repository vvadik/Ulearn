using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public class CSharpSolutionValidator : ISolutionValidator
	{
		private static readonly ICSharpSolutionValidator redundantIf = new RedundantIfStyleValidator();
		private static readonly ICSharpSolutionValidator namingCase = new NamingCaseStyleValidator();

		private readonly List<ICSharpSolutionValidator> validators = new List<ICSharpSolutionValidator> { redundantIf, namingCase };

		public CSharpSolutionValidator AddValidator(ICSharpSolutionValidator validator)
		{
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
				SyntaxTree solutionTree = CSharpSyntaxTree.ParseText(userCode,
					options: new CSharpParseOptions(kind: SourceCodeKind.Script));
				return validators.Select(v => v.FindError(solutionTree)).FirstOrDefault(err => err != null);
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
	}
}