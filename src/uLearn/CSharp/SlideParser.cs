using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public static class SlideParser
	{
		public static Slide ParseSlide(string filename, SlideInfo slideInfo, DirectoryInfo di, CourseSettings settings)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filename));
			return ParseSyntaxTree(tree, slideInfo, "using System; using System.Linq;", di, settings);
		}

		public static Slide ParseCode(string sourceCode, SlideInfo slideInfo, string prelude, DirectoryInfo di, CourseSettings settings)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
			return ParseSyntaxTree(tree, slideInfo, prelude, di, settings);
		}

		private static Slide ParseSyntaxTree(SyntaxTree tree, SlideInfo slideInfo, string prelude,
			DirectoryInfo getInclude, CourseSettings settings)
		{
			var blocksBuilder = new SlideBuilder(getInclude);
			blocksBuilder.Visit(tree.GetRoot());
			if (!ExerciseBuilder.IsExercise(tree))
				return new Slide(blocksBuilder.Blocks, slideInfo, blocksBuilder.Title, blocksBuilder.Id);
			var exerciseBlock = new ExerciseBuilder(SlideBuilder.LangId, prelude).BuildBlockFrom(tree);
			exerciseBlock.CheckScoringGroup(slideInfo.SlideFile.FullName, settings.Scoring);
			blocksBuilder.Blocks.Add(exerciseBlock);
			return new ExerciseSlide(blocksBuilder.Blocks, slideInfo, blocksBuilder.Title, blocksBuilder.Id);
		}
	}
}