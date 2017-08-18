using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class IndentsValidator_Should_Report
	{
		private IndentsValidator indentsValidator = new IndentsValidator();
		private StringBuilder validatorOut = new StringBuilder();

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			indentsValidator.Warning += msg => validatorOut.AppendLine(msg);
		}

		[TearDown]
		public void TearDown()
		{
			validatorOut.Clear();
		}
		private static string notConsistentMsg = "Not consistent indentation between lines: 0, 1";

		public static CodeTestCase[] incorrectTestCases =
		{
			new CodeTestCase
			{
				FileName = "MissingTabIndent",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				FileName = "MissingSpaceIndent",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				FileName = "RedundantTabIndent",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				FileName = "RedundantSpaceIndent",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				FileName = "TabMixedWithSpace",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				FileName = "SpaceIndentsNotConsistent",
				Warning = notConsistentMsg
			},
		};

		[TestCaseSource(nameof(incorrectTestCases))]
		public void On_IncorrectIndents(CodeTestCase testCase)
		{
			var code = IndentsTestHelper.IncorrectTestDataDir.GetFiles($"{testCase.FileName}.cs").Single().ContentAsUtf8();
			indentsValidator.ReportWarningIfIndentsNotValidated(code);
			validatorOut.ToString().Should().Be(testCase.Warning);
		}
	}
}