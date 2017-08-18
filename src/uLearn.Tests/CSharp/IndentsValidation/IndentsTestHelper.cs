using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class IndentsTestHelper
	{
		public static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		public static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		public static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "IndentsValidation", "TestData"));

		[Test]
		[Explicit]
		public void Write_ProjectExercises_UserCodeFile_FullNames_ToTxt()
		{
			var projectExerciseUserCodeFiles =
				new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projectExerciseUserCodeFiles.txt"));
			var courseDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "CSharp",
				"BasicProgramming", "Part01", "BasicProgramming"));
			var course = new CourseLoader().LoadCourse(courseDir);
			var exBlocks = course.Slides
				.Where(slide => slide is ExerciseSlide)
				.SelectMany(exSlide => exSlide.Blocks)
				.OfType<ExerciseBlock>();
			var fullnames = exBlocks.OfType<ProjectExerciseBlock>()
				.Aggregate("", (current, projExBlock) => current + projExBlock.UserCodeFile.FullName + "\r\n");
			File.WriteAllText(projectExerciseUserCodeFiles.FullName, fullnames);
		}
	}
}