using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ApprovalTests;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using test;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class ProjModifier_Should
	{
		private Project CreateTestProject() => new Project(TestProjectFilename, null, null, new ProjectCollection());
		private string TestProjectFilename => Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject", "ProjDir", "test.csproj");
		private string userCodeFileName = $"{nameof(MeaningOfLifeTask)}.cs";
		private string wrongAnswerFileName = $"{nameof(MeaningOfLifeTask)}.WrongAnswer.Type.cs";

		[Test]
		public void NotChangeFile_OnModify()
		{
			ProjModifier.ModifyCsproj(
				new FileInfo(TestProjectFilename), 
				p => ProjModifier.PrepareForChecking(p, "AAA", new string[0]));
			Assert.AreNotEqual("AAA", CreateTestProject().GetProperty("StartupObject"));
		}
		[Test]
		public void ReplaceLinksWithItems()
		{
			var project = CreateTestProject();
			var copies = ProjModifier.ReplaceLinksWithItemsCopiedToProjectDir(project);
			copies.Should().HaveCount(1);
			var writer = new StringWriter();
			foreach (var fileToCopy in copies)
			{
				writer.WriteLine("Copy " + fileToCopy.SourceFile + " to " + fileToCopy.DestinationFile);
			}
			project.Save(writer);
			Approvals.Verify(writer.ToString());
			project.Save(Path.Combine(project.DirectoryPath, "res.csproj"));
		}

		[Test]
		public void SetFilenameItemTypeToCompile()
		{
			var project = CreateTestProject();
			var userCodeItem = project.Items.Single(i => i.UnevaluatedInclude.Equals(userCodeFileName));

			ProjModifier.SetFilenameItemTypeToCompile(project, userCodeFileName);

			userCodeItem.ItemType.Should().Be("Compile");
		}

		[Test]
		public void SetFilenameItemTypeToCompile_When_IncludedToSubDirs()
		{
			var project = CreateTestProject();
			var waItem = project.Items.Single(i => i.UnevaluatedInclude.EndsWith(wrongAnswerFileName));

			ProjModifier.SetFilenameItemTypeToCompile(project, wrongAnswerFileName);

			waItem.ItemType.Should().Be("Compile");
		}

		[Test]
		public void PrepareForStudentZip()
		{
			var project = CreateTestProject();
			var ex = new ProjectExerciseBlock { UserCodeFileName = userCodeFileName };

			ProjModifier.PrepareForStudentZip(project, ex);
			var itemNamesForCompile = project.Items
				.Where(i => i.ItemType.Equals("Compile"))
				.Select(i => i.UnevaluatedInclude)
				.ToList();

			new[] { "Program.cs", userCodeFileName }
				.All(itemNamesForCompile.Contains)
				.Should().BeTrue();
			itemNamesForCompile.Any(n => IsWrongAnswer(n) || IsSolution(n))
				.Should().BeFalse();

			bool IsWrongAnswer(string name) => Regex.IsMatch(name, ex.WrongAnswerPathRegexPattern);
			bool IsSolution(string name) => name.Equals(ex.CorrectSolutionFileName);
		}
	}
}