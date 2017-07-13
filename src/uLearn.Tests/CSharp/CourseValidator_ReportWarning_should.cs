using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using test;
using uLearn.Model.Blocks;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportWarning_should
	{
		private string projSlideFolderPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject");
		private string csProjFilePath => Path.Combine("ProjDir", "test.csproj");

		private string tempSlideFolderPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportWarningTests_Temp_SlideFolder");

		private StringBuilder validatorOut;
		private ProjectExerciseBlock exBlock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(projSlideFolderPath, tempSlideFolderPath);

			exBlock = new ProjectExerciseBlock
			{
				StartupObject = "test.Program",
				UserCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs",
				SlideFolderPath = new DirectoryInfo(tempSlideFolderPath),
				CsProjFilePath = csProjFilePath,
				SupressValidatorMessages = false
			};

			var val = Helper.BuildValidator(Helper.BuildSlide(exBlock), validatorOut = new StringBuilder());
			val.ValidateExercises();
		}

		[Test]
		public void ReportWarning_If_ExerciseFolder_DoesntContain_SolutionFile()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(projSlideFolderPath, tempSlideFolderPath);
			var valOut = new StringBuilder();
			var val = Helper.BuildValidator(Helper.BuildSlide(exBlock), valOut);
			FileSystem.DeleteFile(exBlock.SolutionFile.FullName);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise directory doesn't contain {exBlock.CorrectSolutionFileName}");
		}

		[Test]
		public void ReportWarning_If_WrongAnswer_Verdict_IsCompilationError()
		{
			validatorOut.ToString()
				.Should().Contain($"Code verdict of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.Type.cs) is not OK. " +
								"RunResult = Id: 00000000-0000-0000-0000-000000000000, Verdict: CompilationError");
		}

		[Test]
		public void ReportWarning_If_WrongAnswer_IsSolution()
		{
			validatorOut.ToString()
				.Should().Contain($"Code of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.21.plus.21.cs) is solution!");
		}

		[Test]
		public void Not_ReportWarning_If_WrongAnswer_IsWrongAnswer()
		{
			validatorOut.ToString()
				.Should().NotContain($"{exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.27.cs");
		}
	}
}