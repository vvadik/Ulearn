using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using RunCsJob;
using RunCsJob.Api;
using test;
using uLearn.Extensions;
using uLearn.Model;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
    [TestFixture]
    public class ProjectExerciseBlock_Should
    {
		private DirectoryInfo slideFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject"));
	    private string csProjFilename = "test.csproj";
        private string csProjFilePath => Path.Combine("ProjDir", "test.csproj");
        private string userCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs";
        private ProjectExerciseBlock ex;
        private List<SlideBlock> exBlocks;

        private DirectoryInfo studentExerciseFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Student_SlideFolder"));
        private DirectoryInfo checkerExerciseFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Checker_SlideFolder"));
        private string studentCsProjFilePath => Path.Combine(studentExerciseFolder.FullName, csProjFilename);
        private string checkerExerciseFilePath => Path.Combine(checkerExerciseFolder.FullName, csProjFilename);

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ReCreateFolder(studentExerciseFolder);
            ReCreateFolder(checkerExerciseFolder);

            ex = new ProjectExerciseBlock
            {
				StartupObject = "test.Program",
                UserCodeFileName = userCodeFileName,
                SlideFolderPath = slideFolder,
                CsProjFilePath = csProjFilePath
            };

            CreateStudentZip_AndUnpackResult();
			CreateCheckerZip_AndUnpackResult();
        }

        private void CreateStudentZip_AndUnpackResult()
        {
			if (File.Exists(ex.StudentsZip.FullName))
				File.Delete(ex.StudentsZip.FullName);

            var ctx = new BuildUpContext(ex.SlideFolderPath, CourseSettings.DefaultSettings, null, String.Empty);
            exBlocks = ex.BuildUp(ctx, ImmutableHashSet<string>.Empty).ToList();

            Utils.UnpackZip(ex.StudentsZip.Content(), studentExerciseFolder.FullName);
        }

		private void CreateCheckerZip_AndUnpackResult()
        {
            var zipBytes = ex.GetZipBytesForChecker("i_am_user_code");

            Utils.UnpackZip(zipBytes, checkerExerciseFolder.FullName);
        }

        private void ReCreateFolder(DirectoryInfo dir)
        {
            if (FileSystem.DirectoryExists(dir.FullName))
                FileSystem.DeleteDirectory(dir.FullName, DeleteDirectoryOption.DeleteAllContents);
            FileSystem.CreateDirectory(dir.FullName);
        }

        [Test]
        public void FindSolutionFile_OnBuildUp()
        {
            var correctSolutionCode = ex.SolutionFile.ContentAsUtf8();

            exBlocks.OfType<CodeBlock>()
                .Should().Contain(block => block.Code.Equals(correctSolutionCode) && block.Hide);
        }

        [Test]
		public void When_CreateStudentZip_Contain_UserCodeFile_OfCompileType_Inside_Csproj()
        {
			var itemNamesForCompile = GetFromCsProjItemsForCompile(studentCsProjFilePath);

            itemNamesForCompile.Should().Contain(userCodeFileName);
		}

		[Test]
		public void When_CreateStudentZip_NotContain_AnyWrongAnswersOrSolution_OfCompileType_InsideCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsForCompile(studentCsProjFilePath);

			itemNamesForCompile.Should().NotContain(Helper.WrongAnswersAndSolutionNames);
		}

		[Test]
		public void When_CreateStudentZip_NotContain_AnyWrongAnswersOrSolution_Inside_ExerciseDirectory()
		{
			var projFiles = studentExerciseFolder.GetFiles().Select(f => f.Name);

			projFiles.Should().NotContain(Helper.WrongAnswersAndSolutionNames);
		}

		[Test]
		public void When_CreateStudentZip_Make_Project_Able_To_Compile_If_Project_Depends_On_Many_Tasks()
		{
			var submission = new ProjRunnerSubmission
			{
				Id = "my_id",
				Input = "",
				NeedRun = true,
				ProjectFileName = "test.csproj",
				ZipFileData = ex.StudentsZip.Content()
			};
			var result = SandboxRunner.Run(submission);

			result.CompilationOutput.Should().Be("");
			result.Error.Should().Be("");
			result.Verdict.Should().Be(Verdict.Ok);
		}

		[Test]
		public void When_CreateCheckerZip_Contain_UserCodeFile_OfCompileType_InsideCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsForCompile(checkerExerciseFilePath);

			itemNamesForCompile.Should().Contain(userCodeFileName);
		}

		[Test]
		public void When_CreateCheckerZip_NotContain_CorrectSolution_OfCompileType_InsideCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsForCompile(checkerExerciseFilePath);

			itemNamesForCompile.Should().NotContain(ex.CorrectSolutionFileName);
		}

		[Test]
		public void When_CreateCheckerZip_NotRemove_OtherSolutions_OfCompileType_FromCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsForCompile(checkerExerciseFilePath);
			var anotherSolutionReferencedByCurrentSolution = $"{nameof(AnotherTask)}.Solution.cs";

			itemNamesForCompile.Should().Contain(anotherSolutionReferencedByCurrentSolution);
		}

		private List<string> GetFromCsProjItemsForCompile(string projectFile)
		{
			var csproj = new Project(projectFile, null, null, new ProjectCollection());
			var itemNamesForCompile = csproj.Items
				.Where(i => i.ItemType.Equals("Compile"))
				.Select(i => i.UnevaluatedInclude)
				.ToList();
			return itemNamesForCompile;
		}
	}
}