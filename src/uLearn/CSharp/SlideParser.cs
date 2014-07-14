using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public static class SlideParser
	{
		public static Slide ParseSlide(string filename, SlideInfo slideInfo)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseFile(filename);
			return ParseSyntaxTree(tree, slideInfo, "Using System; Using System.Linq;");
		}

		public static Slide ParseCode(string sourceCode, SlideInfo slideInfo, string usings)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
			return ParseSyntaxTree(tree, slideInfo, usings);
		}

		private static Slide ParseSyntaxTree(SyntaxTree tree, SlideInfo slideInfo, string usings)
		{
			var walker = new SlideWalker();
			var sourceForTestingRoot = walker.Visit(tree.GetRoot());
			if (!walker.IsExercise)
				return new Slide(walker.Blocks, slideInfo, walker.Title);
			return new ExerciseSlide(walker.Blocks, walker.ExerciseInitialCode, walker.ExpectedOutput, walker.Hints, new SolutionForTesting(sourceForTestingRoot, usings), slideInfo, walker.Title);
		}
	}
}