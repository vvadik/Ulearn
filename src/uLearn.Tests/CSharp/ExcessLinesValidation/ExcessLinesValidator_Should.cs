using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using Ulearn.Common.Extensions;

// ReSharper disable InconsistentNaming

namespace uLearn.CSharp.ExcessLinesValidation
{
	[TestFixture]
	internal class ExcessLinesValidator_Should
	{
		private static readonly DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "ExcessLinesValidation", "TestData"));

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
			var errors = new ExcessLinesValidator().FindError(code);
			using (ApprovalResults.ForScenario(filename))
			{
				Approvals.Verify(errors);
			}
		}

		[TestCaseSource(nameof(correctFilenames))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = new ExcessLinesValidator().FindError(code);
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNullOrEmpty();
		}
	}
}