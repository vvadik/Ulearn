using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
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
		private ExerciseSlide exSlide;
		private ProjectExerciseBlock exBlock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			exBlock = new ProjectExerciseBlock
			{
				UserCodeFileName = Helper.UserCodeFileName,
				SlideFolderPath = tempSlideFolder,
				CsProjFilePath = Helper.CsProjFilePath,
			};

			validator = Helper.BuildProjectExerciseValidator(exBlock, exSlide = Helper.BuildSlide(exBlock), validatorOut = new StringBuilder());

			SaveTempZipFileWithFullProject();

			validator.ReportIfStudentsZipHasErrors();
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
		public void Not_ReportError_When_Supress_Validator_Messages_Flag_Is_Set()
		{
			try
			{
				exBlock.SupressValidatorMessages = true;
				var validatorOutStamp = new StringBuilder(validatorOut.ToString());

			validator.ValidateExercises();

			validatorOut.ToString()
				.Should().Be(validatorOutStamp.ToString());
			}
			finally
			{
				exBlock.SupressValidatorMessages = false;
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
	}
}