using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using RunCsJob;
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

		private readonly StringBuilder validatorOut = new StringBuilder();
		private CourseValidator validator;
		private ExerciseSlide exSlide;
		private ProjectExerciseBlock exBlock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			if (FileSystem.DirectoryExists(tempSlideFolderPath))
				FileSystem.DeleteDirectory(tempSlideFolderPath, DeleteDirectoryOption.DeleteAllContents);
			FileSystem.CopyDirectory(projSlideFolderPath, tempSlideFolderPath, true);

			var unit = new Unit(new UnitSettings { Title = "UnitTitle" }, null);
			var slideInfo = new SlideInfo(unit, null, 0);
			exBlock = new ProjectExerciseBlock
			{
				StartupObject = "test.Program",
				UserCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs",
				SlideFolderPath = new DirectoryInfo(tempSlideFolderPath),
				CsProjFilePath = csProjFilePath
			};
			exSlide = new ExerciseSlide(new List<SlideBlock> { exBlock }, slideInfo, "SlideTitle", Guid.Empty);

			validator = new CourseValidator(new List<Slide> { exSlide }, new SandboxRunnerSettings());
			validator.Warning += msg => { validatorOut.Append(msg); };
			validator.Error += msg => { validatorOut.Append(msg); };
			
			FileSystem.DeleteFile(exBlock.SolutionFile.FullName);
			validator.ReportWarningIfExerciseDirDoesntContainSolutionFile(exSlide, exBlock);
			validator.ReportWarningIfWrongAnswersAreSolutionsOrNotOk(exSlide, exBlock);
		}

		[Test]
		public void ReportWarning_If_ExerciseFolder_DoesntContain_SolutionFile()
		{

			validatorOut.ToString()
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