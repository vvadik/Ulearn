using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using RunCsJob;
using test;
using uLearn.Model.Blocks;
// ReSharper disable AssignNullToNotNullAttribute

namespace uLearn.CSharp
{
	[TestFixture]
	public class CourseValidator_ReportError_should
	{
		private string projSlideFolderPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject");
		private string projExerciseFolderPath => Path.Combine(projSlideFolderPath, "ProjDir");
		private DirectoryInfo projExerciseFolder => new DirectoryInfo(projExerciseFolderPath);
		private string csProjFilePath => Path.Combine("ProjDir", "test.csproj");

		private string tempSlideFolderPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "ReportErrorTests_Temp_SlideFolder");
		private FileInfo tempZipFile => new FileInfo(Path.Combine(tempSlideFolderPath, "ProjDir.exercise.zip"));

		private readonly StringBuilder validatorOut = new StringBuilder();
		private CourseValidator validator;
		private ExerciseSlide exSlide;
		private ProjectExerciseBlock exBlock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			SaveTempZipFileWithFullProject();

			var unit = new Unit(new UnitSettings { Title = "UnitTitle" }, null);
			var slideInfo = new SlideInfo(unit, null, 0);
			exBlock = new ProjectExerciseBlock
			{
				UserCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs",
				SlideFolderPath = tempZipFile.Directory,
				CsProjFilePath = csProjFilePath
			};
			exSlide = new ExerciseSlide(new List<SlideBlock> { exBlock }, slideInfo, "SlideTitle", Guid.Empty);

			validator = new CourseValidator(new List<Slide> { exSlide }, new SandboxRunnerSettings());
			validator.Warning += msg => { validatorOut.Append(msg); };
			validator.Error += msg => { validatorOut.Append(msg); };

			validator.ReportIfStudentsZipHasErrors(exSlide, exBlock);
		}

		private void SaveTempZipFileWithFullProject()
		{
			if (FileSystem.DirectoryExists(tempZipFile.DirectoryName))
				FileSystem.DeleteDirectory(tempZipFile.DirectoryName, DeleteDirectoryOption.DeleteAllContents);
			FileSystem.CreateDirectory(tempZipFile.DirectoryName);

			var regexMatchingNothing = new Regex("[^\\s\\S]").ToString();
			new LazilyUpdatingZip(
					projExerciseFolder,
					new string[0],
					regexMatchingNothing,
					_ => null, tempZipFile)
				.UpdateZip();
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
			exBlock.SupressValidatorMessages = true;
			var validatorOutStamp = new StringBuilder(validatorOut.ToString());

			validator.ValidateExercises();

			validatorOut.ToString()
				.Should().Be(validatorOutStamp.ToString());

			exBlock.SupressValidatorMessages = false;
		}
	}
}