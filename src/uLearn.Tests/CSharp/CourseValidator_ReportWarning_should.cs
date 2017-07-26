using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Model;
using uLearn.Model.Blocks;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportWarning_should
	{
		private static string tempSlideFolderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportWarningTests_Temp_SlideFolder");
		private static DirectoryInfo tempSlideFolder = new DirectoryInfo(tempSlideFolderPath);

		private static ProjectExerciseBlock exBlock = new ProjectExerciseBlock
		{
			StartupObject = "test.Program",
			UserCodeFileName = Helper.UserCodeFileName,
			SlideFolderPath = tempSlideFolder,
			CsProjFilePath = Helper.CsProjFilePath
		};

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);

			var ctx = new BuildUpContext(exBlock.SlideFolderPath, CourseSettings.DefaultSettings, null, String.Empty);
			exBlock.BuildUp(ctx, ImmutableHashSet<string>.Empty).ToList();
		}

		[SetUp]
		public void SetUp()
		{
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath, true);
		}

		[Test]
		public void ReportWarning_If_ExerciseFolder_DoesntContain_SolutionFile()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			FileSystem.DeleteFile(exBlock.SolutionFile.FullName);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise directory doesn't contain {exBlock.CorrectSolutionFileName}");
		}

		[Test]
		public void ReportWarning_If_WrongAnswers_Have_Errors()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should()
				.Contain($"Code verdict of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.Type.cs) is not OK.")
				.And
				.Contain("Verdict: CompilationError");
			valOut.ToString()
				.Should().Contain($"Code of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.21.plus.21.cs) is solution!");
			valOut.ToString()
				.Should().NotContain($"{exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.27.cs");
		}
	}
}