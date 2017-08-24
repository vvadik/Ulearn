using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace uLearn.CSharp.IndentsValidation
{
	[TestFixture]
	public class IndentsValidator_Should
	{
		public static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "IndentsValidation", "TestData"));

		private static DirectoryInfo IncorrectTestDataDir => TestDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => TestDataDir.GetDirectories("Correct").Single();

		public static string[] correctFilenames => CorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();
		public static string[] incorrectFilenames => IncorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();

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
		public void NotFindErrors_On_Course_Cs_Files()
		{
			var filepaths = TestDataDir.GetFiles("TestFilepaths.txt").Single().ContentAsUtf8()
				.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			var pathAndCode = filepaths.Select(filepath => Tuple.Create(filepath, File.ReadAllText(filepath)));
			var failed = false;
			foreach (var tuple in pathAndCode.Where(tuple => !tuple.Item1.EndsWith("Settings.Designer.cs") &&
															!tuple.Item1.EndsWith("Resources.Designer.cs")))
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