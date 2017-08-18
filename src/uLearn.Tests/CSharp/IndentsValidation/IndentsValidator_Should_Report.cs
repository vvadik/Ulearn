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
		private static string nestingIndentWarning = "Line 2 has no nesting indent as at line 0";
		private static string redundantIndentWarning = "Line 2 has redundant nesting indent";

		public static CodeTestCase[] incorrectTestCases =
		{
			new CodeTestCase
			{
				FileName = "MissingNestingTabIndent",
				Warning = nestingIndentWarning
			},
			new CodeTestCase
			{
				FileName = "MissingNestingSpaceIndent",
				Warning = nestingIndentWarning
			},
			new CodeTestCase
			{
				FileName = "TabMixedWithSpace",
				Warning = nestingIndentWarning
			},
			new CodeTestCase
			{
				FileName = "RedundantTabIndent",
				Warning = redundantIndentWarning
			},
			new CodeTestCase
			{
				FileName = "RedundantSpaceIndent",
				Warning = redundantIndentWarning
			},
			new CodeTestCase
			{
				FileName = "SpaceIndentsNotConsistent",
				Warning = nestingIndentWarning
			},
			new CodeTestCase
			{
				FileName = "NoAnyIndents",
				Warning = nestingIndentWarning
			}
		};

		[TestCaseSource(nameof(incorrectTestCases))]
		public void On_IncorrectIndents(CodeTestCase testCase)
		{
			var code = IndentsTestHelper.IncorrectTestDataDir.GetFiles($"{testCase.FileName}.cs").Single().ContentAsUtf8();
			var validator = new IndentsValidator(code);
			var validatorOut = new StringBuilder();
			validator.Warning += msg => validatorOut.Append(msg);

			validator.ReportFirstWarningIfIndentsNotValidated();

			validatorOut.ToString().Should().Be(testCase.Warning);
		}
	}
}