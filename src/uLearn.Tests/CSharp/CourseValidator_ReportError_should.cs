using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using uLearn.Extensions;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportError_should
	{
		private string tempSlideFolderPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportErrorTests_Temp_SlideFolder");
		private DirectoryInfo tempSlideFolder => new DirectoryInfo(tempSlideFolderPath);
		private FileInfo tempZipFile => new FileInfo(Path.Combine(tempSlideFolderPath, "ProjDir.exercise.zip"));

		private StringBuilder validatorOut;
		private ProjectExerciseValidator validator;
		private ProjectExerciseBlock exBlock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			exBlock = new ProjectExerciseBlock
			{
				StartupObject = "test.Program",
				UserCodeFileName = Helper.UserCodeFileName,
				SlideFolderPath = tempSlideFolder,
				CsProjFilePath = Helper.CsProjFilePath,
			};

			validator = Helper.BuildProjectExerciseValidator(exBlock, Helper.BuildSlide(exBlock), validatorOut = new StringBuilder());

			SaveTempZipFileWithFullProject();

			validator.ReportErrorIfStudentsZipHasErrors();
		}

		private void SaveTempZipFileWithFullProject()
		{
			Helper.RecreateDirectory(tempZipFile.DirectoryName);

			var noExcludedFiles = new Regex("[^\\s\\S]").ToString();
			var noExcludedDirs = new string[0];
			new LazilyUpdatingZip(
					Helper.ProjExerciseFolder,
					noExcludedDirs,
					noExcludedFiles,
					ResolveCsprojLink,
					tempZipFile)
				.UpdateZip();

			byte[] ResolveCsprojLink(FileInfo file)
				=> file.Name.Equals(exBlock.CsprojFileName) ? ProjModifier.ModifyCsproj(file, ProjModifier.ResolveLinks) : null;
		}

		[Test]
		public void ReportError_If_StudentZip_Has_WrongAnswers_Or_Solution_Files()
		{
			validatorOut.ToString()
				.Should().Contain($"Student zip exercise directory has 'wrong answer' and/or solution files ({Helper.OrderedWrongAnswersAndSolutionNames})");
		}

		[Test]
		public void ReportError_If_Student_Csproj_Has_UserCodeFile_Of_Not_CompileType()
		{
			validatorOut.ToString()
				.Should().Contain($"Student's csproj has user code item ({exBlock.UserCodeFileName}) of not compile type");
		}

		[Test]
		public void ReportError_If_Student_Csproj_Has_WrongAnswers_Or_Solution_Items()
		{
			validatorOut.ToString()
				.Should().Contain($"Student's csproj has 'wrong answer' and/or solution items ({Helper.OrderedWrongAnswersAndSolutionNames})");
		}

		[Test]
		public void Not_Report_InitialCodeIsSolution_Error_When_DisableUserCodeFileValidations_Flag_Is_On()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);
			var correctSolution = new FileInfo(Path.Combine(exBlock.ExerciseFolder.FullName, exBlock.CorrectSolutionFileName));
			var tempUserCode = exBlock.UserCodeFile.ContentAsUtf8();
			var valOut = new StringBuilder();
			try
			{
				File.WriteAllText(exBlock.UserCodeFile.FullName, correctSolution.ContentAsUtf8());
				exBlock.DisableUserCodeFileValidations = true;
				var val = Helper.BuildValidator(Helper.BuildSlide(exBlock), valOut);

				val.ValidateExercises();

				valOut.ToString().Should().NotContain("Exercise initial code (available to students) is solution!");
			}
			finally
			{
				File.WriteAllText(exBlock.UserCodeFile.FullName, tempUserCode);
				exBlock.DisableUserCodeFileValidations = false;
			}
		}

		[Test]
		public void Report_InitialCodeIsSolution_Error_When_DisableUserCodeFileValidations_Flag_Is_Off()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);
			var correctSolution = new FileInfo(Path.Combine(exBlock.ExerciseFolder.FullName, exBlock.CorrectSolutionFileName));
			var tempUserCode = exBlock.UserCodeFile.ContentAsUtf8();
			var valOut = new StringBuilder();
			try
			{
				File.WriteAllText(exBlock.UserCodeFile.FullName, correctSolution.ContentAsUtf8());
				var val = Helper.BuildValidator(Helper.BuildSlide(exBlock), valOut);

				val.ValidateExercises();

				valOut.ToString().Should().Contain("Exercise initial code (available to students) is solution!");
			}
			finally
			{
				File.WriteAllText(exBlock.UserCodeFile.FullName, tempUserCode);
			}
		}

		[Test]
		public void ReportError_If_ExerciseFolder_Doesnt_Contain_CsProj()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);
			var valOut = new StringBuilder();
			var val = Helper.BuildValidator(Helper.BuildSlide(exBlock), valOut);
			File.Delete(Path.Combine(tempSlideFolderPath, exBlock.CsProjFilePath));

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise folder ({exBlock.ExerciseFolder.Name}) doesn't contain ({exBlock.CsprojFileName})");
		}

		[Test]
		public void ReportError_If_ExerciseFolder_Doesnt_Contain_UserCodeFile()
		{
			Helper.RecreateDirectory(tempSlideFolderPath);
			FileSystem.CopyDirectory(Helper.ProjSlideFolderPath, tempSlideFolderPath);
			var valOut = new StringBuilder();
			var val = Helper.BuildValidator(Helper.BuildSlide(exBlock), valOut);
			File.Delete(exBlock.UserCodeFile.FullName);

			val.ValidateExercises();

			valOut.ToString()
				.Should().Contain($"Exercise folder ({exBlock.ExerciseFolder.Name}) doesn't contain ({exBlock.UserCodeFileName})");
		}

		[Test]
		public void ReportError_If_Ethalon_Solution_For_ProjectExerciseBlock_IsNot_Correct()
		{
			throw new NotImplementedException();
		}
	}
}