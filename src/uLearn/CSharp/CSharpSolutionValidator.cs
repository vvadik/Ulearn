using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public class CSharpSolutionValidator : ISolutionValidator
	{
		private readonly List<ICSharpSolutionValidator> validators = new List<ICSharpSolutionValidator>();

		public void AddValidator(ICSharpSolutionValidator validator)
		{
			validators.Add(validator);
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