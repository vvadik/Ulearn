using System;
using System.IO;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class ProjModifier_Should
	{
		private Project CreateTestProject() => new Project(TestProjectFilename, null, null, new ProjectCollection());
		private string TestProjectFilename => Path.Combine(TestContext.CurrentContext.TestDirectory, "CSharp", "TestProject", "ProjDir", "test.csproj");

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
	}
}