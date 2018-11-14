using System.Collections.Immutable;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using uLearn.Courses;
using uLearn.Courses.Slides;
using uLearn.Courses.Slides.Blocks;
using Ulearn.Common.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportWarning_should
	{
		private static string tempSlideFolderPath = Path.Combine(TestContext.CurrentContext.TestDirectory,
			"ReportWarningTests_Temp_SlideFolder");

		private static DirectoryInfo tempSlideFolder = new DirectoryInfo(tempSlideFolderPath);

		private static ProjectExerciseBlock exBlock = new ProjectExerciseBlock
		{
			StartupObject = "test.Program",
			UserCodeFilePath = TestsHelper.UserCodeFileName,
			SlideFolderPath = tempSlideFolder,
			CsProjFilePath = TestsHelper.CsProjFilePath
		};

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			TestsHelper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(TestsHelper.ProjSlideFolderPath, tempSlideFolderPath);

			string studentZipFilepath = Path.Combine(tempSlideFolderPath, "ProjDir.exercise.zip");
			if (File.Exists(studentZipFilepath))
				File.Delete(studentZipFilepath);

			var ctx = new BuildUpContext(new Unit(null, exBlock.SlideFolderPath), CourseSettings.DefaultSettings, null, "Test", string.Empty);
			exBlock.BuildUp(ctx, ImmutableHashSet<string>.Empty).ToList();
		}

		[SetUp]
		public void SetUp()
		{
			FileSystem.CopyDirectory(TestsHelper.ProjSlideFolderPath, tempSlideFolderPath, true);
		}

		[Test]
		public void ReportWarning_If_ExerciseFolder_DoesntContain_SolutionFile()
		{
			FileSystem.DeleteFile(exBlock.CorrectSolutionFile.FullName);

			var validatorOut = TestsHelper.ValidateBlock(exBlock);

			validatorOut
				.Should().Contain($"Exercise directory doesn't contain {exBlock.CorrectSolutionFileName}");
		}

		[Test]
		public void ReportWarning_If_WrongAnswers_Have_Errors()
		{
			var validatorOut = TestsHelper.ValidateBlock(exBlock);

			validatorOut
				.Should()
				.Contain(
					$"Code verdict of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.Type.cs) is not OK.")
				.And
				.Contain("Verdict: CompilationError");
			validatorOut
				.Should().Contain(
					$"Code of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.21.plus.21.cs) is solution!");
			validatorOut
				.Should().NotContain($"{exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.27.cs");
		}

		[Test]
		public void Not_Report_Indentation_Warning_On_Ethalon_Solution_Of_SingleFileExercise()
		{
			var singleBlock =
				new ExerciseBuilder("cs", "using System; using System.Linq; using System.Text;").BuildBlockFrom(
					CSharpSyntaxTree.ParseText(TestsHelper.ProjSlideFolder.GetFile("S055 - Упражнение на параметры по умолчанию.cs")
						.ContentAsUtf8()), null);
			var validatorOut = TestsHelper.ValidateBlock(singleBlock);
			validatorOut.Should().BeNullOrEmpty();
		}
	}
}