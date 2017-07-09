using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using test;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class LazilyUpdatingZip_should
	{
		private string projectDirPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject", "ProjDir");
		private DirectoryInfo projectDir => new DirectoryInfo(projectDirPath);
		private ProjectExerciseBlock ex = new ProjectExerciseBlock { UserCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs" };

		private LazilyUpdatingZip zip => new LazilyUpdatingZip(
			projectDir,
			new[] { "checking" }, new[] { ex.CorrectSolutionFileName },
			ex.WrongAnswerPathRegexPattern,
			null, null);

		[Test]
		public void Exclude_WrongAnswers_OnEnumerateFiles()
		{
			var mustBeExluded = new[]
			{
				$"{ex.UserCodeFileNameWithoutExt}.WrongAnswer.27.cs",
				$"{ex.UserCodeFileNameWithoutExt}.WrongAnswer.21.plus.21.cs",
			};

			zip.EnumerateFiles()
				.Select(f => f.Name)
				.Any(mustBeExluded.Contains)
				.Should().BeFalse();
		}

		[Test]
		public void Exclude_WrongAnswers_FromSubDirs_OnEnumerateFiles()
		{
			var mustBeExluded = new[]
			{
				$"{ex.UserCodeFileNameWithoutExt}.WrongAnswer.Type.cs",
				$"{ex.UserCodeFileNameWithoutExt}.WrongAnswer.25.cs",
			};

			zip.EnumerateFiles()
				.Select(f => f.Name)
				.Any(mustBeExluded.Contains)
				.Should().BeFalse();
		}

		[Test]
		public void Exclude_Solution_OnEnumerateFiles()
		{
			zip.EnumerateFiles()
				.Any(f => f.Name.Equals($"{ex.UserCodeFileNameWithoutExt}.Solution.cs"))
				.Should().BeFalse();
		}

		[Test]
		public void Exclude_FilesFromExcludedDirs_OnEnumerateFiles()
		{
			zip.EnumerateFiles()
				.Any(f => f.Name.Equals($"{ex.UserCodeFileNameWithoutExt}.Factory.cs"))
				.Should().BeFalse();
		}

		[Test]
		public void Include_FilesFor_StudentZip_OnEnumerateFiles()
		{
			var mustBeIncluded = new[]
			{
				$"{ex.UserCodeFileName}",
				"Program.cs"
			};

			var forStudent = zip.EnumerateFiles().Select(f1 => f1.Name).ToList();

			mustBeIncluded
				.All(forStudent.Contains)
				.Should().BeTrue();
		}
	}
}