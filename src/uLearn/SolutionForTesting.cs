using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace uLearn
{
	public class SolutionForTesting
	{
		private readonly int indexForInsert;
		private readonly string preparedSlide;
		private readonly List<string> allUsings;
		private readonly string defaultUsings;

		public SolutionForTesting(SyntaxNode sourceForTestingRoot, string usings)
		{
			defaultUsings = usings;
			preparedSlide = sourceForTestingRoot.ToFullString();
			allUsings = sourceForTestingRoot.DescendantNodes().Where(x => x.CSharpKind() == SyntaxKind.UsingDirective).Select(x => x.ToString()).ToList();
			var classDeclaration = sourceForTestingRoot.DescendantNodes()
				.OfType<ClassDeclarationSyntax>().First();
			indexForInsert = classDeclaration.OpenBraceToken.Span.End;
		}

		public string BuildSolution(string usersExercise)
		{
			var solution =  preparedSlide.Insert(indexForInsert, "\r\n#line 1\r\n" + usersExercise + "\r\n");
			solution = allUsings.Aggregate(solution, (current, usings) => current.Replace(usings, ""));
			return defaultUsings + solution;
		}
	}
}
