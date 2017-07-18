using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Model.Blocks;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportWarning_should
	{
		private static string tempSlideFolderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportWarningTests_Temp_SlideFolder");

		private static ProjectExerciseBlock exBlock = new ProjectExerciseBlock
			{
				StartupObject = "test.Program",
				UserCodeFileName = Helper.UserCodeFileName,
				SlideFolderPath = new DirectoryInfo(tempSlideFolderPath),
				CsProjFilePath = Helper.CsProjFilePath
			};

		[SetUp]
		public void SetUp()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);
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
		public void ReportWarning_If_WrongAnswer_Verdict_IsCompilationError()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should()
				.Contain($"Code verdict of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.Type.cs) is not OK.")
				.And
				.Contain("Verdict: CompilationError");
		}

		[Test]
		public void ReportWarning_If_WrongAnswer_IsSolution()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Code of file with wrong answer ({exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.21.plus.21.cs) is solution!");
		}

		[Test]
		public void Not_ReportWarning_If_WrongAnswer_IsWrongAnswer()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should().NotContain($"{exBlock.UserCodeFileNameWithoutExt}.WrongAnswer.27.cs");
		}
	}
}