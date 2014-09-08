using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using uLearn.CSharp;

namespace uLearn
{
	public class SolutionBuilder
	{
		private readonly int indexForInsert;
		private readonly string preparedSlide;
		private readonly string prelude;
		public readonly string TemplateSolution;
		private readonly List<ISolutionValidator> validators;

		public SolutionBuilder(SyntaxNode sourceForTestingRoot, string prelude, List<ISolutionValidator> validators, string templateSolution, ClassDeclarationSyntax solutionClass)
		{
			this.prelude = prelude;
			this.validators = validators;
			preparedSlide = sourceForTestingRoot.ToFullString();
			var classDeclaration = sourceForTestingRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
			indexForInsert = classDeclaration.OpenBraceToken.Span.End;
			TemplateSolution = templateSolution;
		}

		public SolutionBuildResult BuildSolution(string usersExercise)
		{
			var solution =  prelude + preparedSlide.Insert(indexForInsert, "\r\n#line 1\r\n" + usersExercise + "\r\n");
			return FindSyntaxError(solution) 
				?? FindValidatorError(usersExercise) 
				?? SolutionBuildResult.Success(solution);
		}

		private static SolutionBuildResult FindSyntaxError(string solution)
		{
			IEnumerable<Diagnostic> diagnostics = CSharpSyntaxTree.ParseText(solution).GetDiagnostics();
			var error = diagnostics.FirstOrDefault();
			return error != null ? SolutionBuildResult.Error(error.ToString()) : null;
		}

		private SolutionBuildResult FindValidatorError(string usersExercise)
		{
			try
			{
				SyntaxTree solutionTree = CSharpSyntaxTree.ParseText(usersExercise,
					options: new CSharpParseOptions(kind: SourceCodeKind.Script));
				var error = validators.Select(v => v.FindError(solutionTree)).FirstOrDefault(err => err != null);
				return error != null ? SolutionBuildResult.Error(error) : null;
			}
			catch (Exception e)
			{
				return SolutionBuildResult.Error(e.Message);
			}
		}
	}
}
