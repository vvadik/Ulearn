using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public static class SlideParser
	{
		public static Slide ParseSlide(string filename, SlideInfo slideInfo)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseFile(filename);
			return ParseSyntaxTree(tree, slideInfo);
		}

		public static Slide ParseCode(string sourceCode, SlideInfo slideInfo)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
			return ParseSyntaxTree(tree, slideInfo);
		}

		private static Slide ParseSyntaxTree(SyntaxTree tree, SlideInfo slideInfo)
		{
			var walker = new SlideWalker();
			var sourceForTestingRoot = walker.Visit(tree.GetRoot());
			if (!walker.IsExercise)
				return new Slide(walker.Blocks, slideInfo, walker.Title);
			return new ExerciseSlide(walker.Blocks, walker.ExerciseInitialCode, walker.ExpectedOutput, walker.Hints, new SolutionForTesting(sourceForTestingRoot), slideInfo, walker.Title);
		}
	}
}