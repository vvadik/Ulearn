using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using Ulearn.Common.Extensions;

namespace uLearn.CSharp.TryPatternValidation
{
	[TestFixture]
	internal class TryPatternValidator_should
	{
		private static readonly DirectoryInfo testDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "TryPatternValidation", "TestData"));

		private static DirectoryInfo IncorrectTestDataDir => testDataDir.GetDirectories("Incorrect").Single();
		private static DirectoryInfo CorrectTestDataDir => testDataDir.GetDirectories("Correct").Single();

		private static string[] CorrectFilenames => CorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();
		private static string[] IncorrectFilenames => IncorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();

		[Test]
		[TestCaseSource(nameof(IncorrectFilenames))]
		[UseReporter(typeof(DiffReporter))]
		public void FindErrors(string filename)
		{
			var code = IncorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new TryPatternValidator().FindError(code);
			using (ApprovalResults.ForScenario(filename))
			{
				Approvals.Verify(errors);
			}
		}

		[TestCaseSource(nameof(CorrectFilenames))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new TryPatternValidator().FindError(code);
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNullOrEmpty();
		}
	}
}