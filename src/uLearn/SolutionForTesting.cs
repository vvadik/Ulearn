using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn
{
	public class SolutionForTesting
	{
		private readonly int indexForInsert;
		private readonly string preparedSlide;

		public SolutionForTesting(SyntaxNode sourceForTestingRoot)
		{
			preparedSlide = sourceForTestingRoot.ToFullString();
			var classDeclaration = sourceForTestingRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
			indexForInsert = classDeclaration.OpenBraceToken.Span.End;
		}

		public string BuildSolution(string usersExercise)
		{
			return preparedSlide.Insert(indexForInsert, "\r\n#line 1\r\n" + usersExercise + "\r\n");
		}
	}
}
