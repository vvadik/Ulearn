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
        private ProjectExerciseBlock ex;
        private List<SlideBlock> exBlocks;

        private DirectoryInfo studentExerciseFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Student_ExerciseFolder"));
        private DirectoryInfo checkerExerciseFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Checker_ExerciseFolder"));
        private string studentCsProjFilePath => Path.Combine(studentExerciseFolder.FullName, Helper.CsProjFilename);
        private string checkerCsprojFilePath => Path.Combine(checkerExerciseFolder.FullName, Helper.CsProjFilename);

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ReCreateFolder(studentExerciseFolder);
            ReCreateFolder(checkerExerciseFolder);

            ex = new ProjectExerciseBlock
            {
				StartupObject = "test.Program",
                UserCodeFileName = Helper.UserCodeFileName,
                SlideFolderPath = Helper.ProjSlideFolder,
                CsProjFilePath = Helper.CsProjFilePath
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
			var itemNamesForCompile = GetFromCsProjItemsNamesForCompile(studentCsProjFilePath);

            itemNamesForCompile.Should().Contain(Helper.UserCodeFileName);
		}

        [Test]
		public void When_CreateStudentZip_Contain_Resolved_Links_Inside_Csproj()
        {
	        var itemNamesForCompile = GetFromCsProjItemsForCompile(checkerCsprojFilePath);

	        itemNamesForCompile.Should().Contain(i => i.UnevaluatedInclude.Equals("~$Link.cs"));
		}

        [Test]
		public void When_CreateStudentZip_Contain_Resolved_Link_Files_Inside_Csproj()
        {
	        var projFiles = studentExerciseFolder.GetFiles().Select(f => f.Name);

	        projFiles.Should().Contain("~$Link.cs");
		}

		[Test]
		public void When_CreateStudentZip_NotContain_AnyWrongAnswersOrSolution_OfCompileType_InsideCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsNamesForCompile(studentCsProjFilePath);

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
			var itemNamesForCompile = GetFromCsProjItemsNamesForCompile(checkerCsprojFilePath);

			itemNamesForCompile.Should().Contain(Helper.UserCodeFileName);
		}

		[Test]
		public void When_CreateCheckerZip_NotContain_CorrectSolution_OfCompileType_InsideCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsNamesForCompile(checkerCsprojFilePath);

			itemNamesForCompile.Should().NotContain(ex.CorrectSolutionFileName);
		}

		[Test]
		public void When_CreateCheckerZip_NotRemove_OtherSolutions_OfCompileType_FromCsproj()
		{
			var itemNamesForCompile = GetFromCsProjItemsNamesForCompile(checkerCsprojFilePath);
			var anotherSolutionReferencedByCurrentSolution = $"{nameof(AnotherTask)}.Solution.cs";

			itemNamesForCompile.Should().Contain(anotherSolutionReferencedByCurrentSolution);
		}

		private List<string> GetFromCsProjItemsNamesForCompile(string projectFile)
		{
			return GetFromCsProjItemsForCompile(projectFile).Select(i => i.UnevaluatedInclude).ToList();
		}

		private List<ProjectItem> GetFromCsProjItemsForCompile(string projectFile)
		{
			var csproj = new Project(projectFile, null, null, new ProjectCollection());
			return csproj.Items
				.Where(i => i.ItemType.Equals("Compile"))
				.ToList();
		}
	}
}