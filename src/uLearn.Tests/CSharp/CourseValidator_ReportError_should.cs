using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using test;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportError_should
	{
		private static string tempSlideFolderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportErrorTests_Temp_SlideFolder");
		private static DirectoryInfo tempSlideFolder = new DirectoryInfo(tempSlideFolderPath);
		private static FileInfo zipWithFullProj = new FileInfo(Path.Combine(tempSlideFolderPath, "ProjDir.exercise.zip"));

		private static ProjectExerciseBlock exBlock = new ProjectExerciseBlock
		{
			StartupObject = "test.Program",
			UserCodeFileName = Helper.UserCodeFileName,
			SlideFolderPath = tempSlideFolder,
			CsProjFilePath = Helper.CsProjFilePath,
		};

		[SetUp]
		public void SetUp()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);
		}

		[Test]
		public void ReportError_If_StudentZip_Has_WrongAnswers_Or_Solution_Files()
		{
			SaveTempZipFileWithFullProject();
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Student zip exercise directory has 'wrong answer' and/or solution files ({Helper.OrderedWrongAnswersAndSolutionNames})");
		}

		[Test]
		public void ReportError_If_Student_Csproj_Has_UserCodeFile_Of_Not_CompileType()
		{
			SaveTempZipFileWithFullProject();
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Student's csproj has user code item ({exBlock.UserCodeFileName}) of not compile type");
		}

		[Test]
		public void ReportError_If_Student_Csproj_Has_WrongAnswers_Or_Solution_Items()
		{
			SaveTempZipFileWithFullProject();
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Student's csproj has 'wrong answer' and/or solution items ({Helper.OrderedWrongAnswersAndSolutionNames})");
		}

		private void SaveTempZipFileWithFullProject()
		{
			var noExcludedFiles = new Regex("[^\\s\\S]").ToString();
			var noExcludedDirs = new string[0];
			new LazilyUpdatingZip(
					Helper.ProjExerciseFolder,
					noExcludedDirs,
					noExcludedFiles,
					ResolveCsprojLink,
					zipWithFullProj)
				.UpdateZip();

			byte[] ResolveCsprojLink(FileInfo file)
				=> file.Name.Equals(exBlock.CsprojFileName) ? ProjModifier.ModifyCsproj(file, ProjModifier.ResolveLinks) : null;
		}

		[Test]
		public void ReportError_If_ExerciseFolder_Doesnt_Contain_CsProj()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			File.Delete(Path.Combine(tempSlideFolderPath, exBlock.CsProjFilePath));

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise folder ({exBlock.ExerciseFolder.Name}) doesn't contain ({exBlock.CsprojFileName})");
		}

		[Test]
		public void ReportError_If_ExerciseFolder_Doesnt_Contain_UserCodeFile()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			File.Delete(exBlock.UserCodeFile.FullName);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise folder ({exBlock.ExerciseFolder.Name}) doesn't contain ({exBlock.UserCodeFileName})");
		}

		[Test]
		public void ReportError_If_Solution_For_ProjectExerciseBlock_Not_Building()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			File.WriteAllText(exBlock.CorrectSolution.FullName, "");

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Correct solution file {exBlock.CorrectSolutionFileName} has errors");
		}

		[Test]
		public void ReportError_If_Solution_For_ProjectExerciseBlock_Not_Passes_NonExisting_Test()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			try
			{
				exBlock.NUnitTestClasses = new[] { "non_existing.test_class", };

				val.ValidateExercises();

				valOut.ToString()
					.Should()
					.Contain($"Correct solution file {exBlock.CorrectSolutionFileName} has errors")
					.And
					.Contain($"test class {exBlock.NUnitTestClasses[0]} does not exist");
			}
			finally
			{
				exBlock.NUnitTestClasses = null;
			}
		}

		[Test]
		public void ReportError_If_Solution_For_ProjectExerciseBlock_Not_Passes_Test()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			try
			{
				exBlock.NUnitTestClasses = new[] { $"test.{nameof(OneFailingTest)}" };

				val.ValidateExercises();

				valOut.ToString()
					.Should()
					.Contain($"Correct solution file {exBlock.CorrectSolutionFileName} has errors")
					.And
					.Contain("Error on NUnit test: I_am_a_failure");
			}
			finally
			{
				exBlock.NUnitTestClasses = null;
			}
		}
	}
}