using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
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
        private string csProjFilePath => Path.Combine("ProjDir", "test.csproj");
        private string userCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs";
        private ProjectExerciseBlock ex;
        private List<SlideBlock> exBlocks;

        private DirectoryInfo studentExerciseFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Student_SlideFolder"));
        private DirectoryInfo checkerExerciseFolder => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Checker_SlideFolder"));
        private string studentCsProjFilePath => Path.Combine(studentExerciseFolder.FullName, "test.csproj");
        private string checkerExerciseFilePath => Path.Combine(checkerExerciseFolder.FullName, "test.csproj");

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ReCreateFolder(studentExerciseFolder);
            ReCreateFolder(checkerExerciseFolder);

            ex = new ProjectExerciseBlock
            {
                UserCodeFileName = userCodeFileName,
                SlideFolderPath = slideFolder,
                CsProjFilePath = csProjFilePath
            };

            CreateStudentZip_AndUnpackResult();
            CreateZipForChecker_AndUnpackResult();
        }

        private void CreateStudentZip_AndUnpackResult()
        {
            var ctx = new BuildUpContext(ex.SlideFolderPath, CourseSettings.DefaultSettings, null, String.Empty);
            exBlocks = ex.BuildUp(ctx, ImmutableHashSet<string>.Empty).ToList();

            Utils.UnpackZip(ex.StudentsZip.Content(), studentExerciseFolder.FullName);
        }

        private void CreateZipForChecker_AndUnpackResult()
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
        public void When_CreateStudentZip_Have_Csproj_WithCorrectItems_OfCompileType()
        {
            var csproj = new Project(studentCsProjFilePath, null, null, new ProjectCollection());
            var itemNamesForCompile = csproj.Items
                .Where(i => i.ItemType.Equals("Compile"))
                .Select(i => i.UnevaluatedInclude)
                .ToList();

            itemNamesForCompile.Should().Contain(userCodeFileName);
            itemNamesForCompile.Any(IsWrongAnswerOrSoltion)
                .Should().BeFalse();
        }

        bool IsWrongAnswerOrSoltion(string name) => Regex.IsMatch(name, ex.WrongAnswersAndSolutionNameRegexPattern);

        [Test]
        public void When_CreateStudentZip_Have_ExerciseDirectory_With_CorrectFiles()
        {
            var projFiles = studentExerciseFolder.GetAllFiles();

            projFiles.Should().NotContain(f => IsWrongAnswerOrSoltion(f.Name));
        }

        [Test]
        public void When_CreateCheckerZip_Have_Csproj_WithCorrectItems_OfCompileType()
        {
            var csproj = new Project(checkerExerciseFilePath, null, null, new ProjectCollection());
            var itemNamesForCompile = csproj.Items
                .Where(i => i.ItemType.Equals("Compile"))
                .Select(i => i.UnevaluatedInclude)
                .ToList();

            itemNamesForCompile.Should().Contain(userCodeFileName);
            itemNamesForCompile.Any(IsWrongAnswerOrSoltion)
                .Should().BeFalse();
        }

        [Test]
        public void When_CreateCheckerZip_Have_ExerciseDirectory_With_CorrectFiles()
        {
            var projFiles = checkerExerciseFolder.GetAllFiles();

            projFiles.Should().NotContain(f => IsWrongAnswerOrSoltion(f.Name));
        }
    }
}