using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;


namespace uLearn.CSharp.IndentsValidation
{
	[TestFixture]
	public class IndentsValidator_Should
	{
		private static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "IndentsValidation", "TestData"));

		private static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		private static string[] correctFilenames => CorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();
		private static string[] incorrectFilenames => IncorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();

		[Test]
		[TestCaseSource(nameof(incorrectFilenames))]
		[UseReporter(typeof(DiffReporter))]
		public void FindErrors(string filename)
		{
			var code = IncorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new IndentsValidator().FindError(code);
			using (ApprovalResults.ForScenario(filename))
			{
				Approvals.Verify(errors);
			}
		}

		[TestCaseSource(nameof(correctFilenames))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new IndentsValidator().FindError(code);
			errors.Should().BeNullOrEmpty();
		}

		[Test]
		[Explicit]
		public void NotFindErrors_On_Cs_Files_Of_Folder()
		{
			var folder = new DirectoryInfo("d:\\BP1_UserCodeFiles\\");
			var filesCode = folder.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => !f.Name.Equals("Settings.Designer.cs") && !f.Name.Equals("Resources.Designer.cs"))
				.Select(f => Tuple.Create(f.FullName, f.ContentAsUtf8()));
			var failed = false;
			foreach (var tuple in filesCode)
			{
				var errors = new IndentsValidator().FindError(tuple.Item2);

				if (errors != null)
				{
					failed = true;
					Console.WriteLine(tuple.Item1);
					Console.WriteLine(errors);
					Console.WriteLine();
				}
			}
			if (failed)
				Assert.Fail();
		}
	}
}