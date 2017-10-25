using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp.BoolCompareValidation
{
	[TestFixture]
	public class BoolCompareValidator_should
	{
		private BoolCompareValidator validator;

		[SetUp]
		public void SetUp()
		{
			validator = new BoolCompareValidator();
		}

		private static DirectoryInfo TestDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "BoolCompareValidation", "TestData"));

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
			var errors = validator.FindError(code);
			errors.Should().NotBeNullOrEmpty();
		}

		[Test]
		[TestCaseSource(nameof(correctFilenames))]
		[UseReporter(typeof(DiffReporter))]
		public void NotFindErrors(string filename)
		{
			var code = CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var errors = validator.FindError(code);
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNullOrEmpty();
		}


		private static DirectoryInfo TestULearnDataDir = new DirectoryInfo("C:\\Users\\smprivalov\\Downloads\\BasicProgramming-master\\BasicProgramming-master");
		private static string[] UlearnCorrectFilenames => 
			TestULearnDataDir.GetFiles("*.cs", SearchOption.AllDirectories ).Select(f => f.Name).ToArray();

		[Test]
		[TestCaseSource(nameof(UlearnCorrectFilenames))]
		[UseReporter(typeof(DiffReporter))]
		public void CheckULearn(string filename)
		{
			var code = TestULearnDataDir.GetFiles(filename, SearchOption.AllDirectories).First().ContentAsUtf8();
			var errors = validator.FindError(code);
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNullOrEmpty();
		}
	}
}