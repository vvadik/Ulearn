using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using test;
using uLearn.Model;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportError_should
	{
		private static string tempSlideFolderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportErrorTests_Temp_SlideFolder");
		private static DirectoryInfo tempSlideFolder = new DirectoryInfo(tempSlideFolderPath);

		private static ProjectExerciseBlock exBlock = new ProjectExerciseBlock
		{
			StartupObject = "test.Program",
			UserCodeFileName = Helper.UserCodeFileName,
			SlideFolderPath = tempSlideFolder,
			CsProjFilePath = Helper.CsProjFilePath,
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
		public void ReportError_If_StudentZip_HasErrors()
		{
			try
			{
				FileSystem.RenameDirectory(tempSlideFolder.GetDirectories("projDir").Single().FullName, "FullProjDir");
				exBlock.CsProjFilePath = Path.Combine("FullProjDir", Helper.CsProjFilename);
				SaveTempZipFileWithFullProject();

				var valOut = new StringBuilder();
				var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);

				val.ValidateExercises();

				valOut.ToString()
					.Should().Contain($"Student zip exercise directory has 'wrong answer' and/or solution files ({Helper.OrderedWrongAnswersAndSolutionNames})");
				valOut.ToString()
					.Should().Contain($"Student's csproj has user code item ({exBlock.UserCodeFileName}) of not compile type");
				valOut.ToString()
					.Should().Contain($"Student's csproj has 'wrong answer' and/or solution items ({Helper.OrderedWrongAnswersAndSolutionNames})");
			}
			finally
			{
				FileSystem.RenameDirectory(tempSlideFolder.GetDirectories("FullProjDir").Single().FullName, "projDir");
				exBlock.CsProjFilePath = Helper.CsProjFilePath;
			}
		}

		private void SaveTempZipFileWithFullProject()
		{
			var zipWithFullProj = new FileInfo(Path.Combine(tempSlideFolderPath, "FullProjDir.exercise.zip"));
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
		public void ReportError_If_ExerciseFolder_HasErrors()
		{
			var valOut = new StringBuilder();
			var val = Helper.BuildProjectExerciseValidator(exBlock, valOut);
			File.Delete(exBlock.UserCodeFile.FullName);
			File.Delete(Path.Combine(tempSlideFolderPath, exBlock.CsProjFilePath));

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise folder ({exBlock.ExerciseFolder.Name}) doesn't contain ({exBlock.CsprojFileName})");
			valOut.ToString()
				.Should().Contain($"Exercise folder ({exBlock.ExerciseFolder.Name}) doesn't contain ({exBlock.UserCodeFileName})");
		}

		[Test]
		public void ReportError_If_CorrectSolution_Has_Errors()
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