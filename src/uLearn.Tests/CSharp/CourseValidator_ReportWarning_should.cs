using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using uLearn.Model;
using uLearn.Model.Blocks;

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
			UserCodeFilePath = TestsHelper.UserCodeFileName,
			SlideFolderPath = tempSlideFolder,
			CsProjFilePath = TestsHelper.CsProjFilePath
		};

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			TestsHelper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(TestsHelper.ProjSlideFolderPath, tempSlideFolderPath);

			var ctx = new BuildUpContext(new Unit(null, exBlock.SlideFolderPath), CourseSettings.DefaultSettings, null, String.Empty);
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
				.Contain($"Code verdict of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.Type.cs) is not OK.")
				.And
				.Contain("Verdict: CompilationError");
			validatorOut
				.Should().Contain($"Code of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.21.plus.21.cs) is solution!");
			validatorOut
				.Should().NotContain($"{exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.27.cs");
		}
	}
}