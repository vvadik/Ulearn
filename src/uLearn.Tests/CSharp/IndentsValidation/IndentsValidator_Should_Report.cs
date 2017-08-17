using System.Text;
using FluentAssertions;
using NUnit.Framework;

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

		public static CodeTestCase[] notConsistentindents =
		{
			new CodeTestCase
			{
				Name = "missing tab indent",
				Code = @"	using System;
namespace uLearn.CSharp",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				Name = "missing space indent",
				Code = @"	using System;
    namespace uLearn.CSharp",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				Name = "redundant tab indent",
				Code = @"using System;
	namespace uLearn.CSharp",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				Name = "redundant space indent",
				Code = @"using System;
    namespace uLearn.CSharp",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				Name = "tab mixed with space",
				Code = @"	using System;
    namespace uLearn.CSharp",
				Warning = notConsistentMsg
			},
			new CodeTestCase
			{
				Name = "space indents not consistent",
				Code = @"    using System;
  namespace uLearn.CSharp",
				Warning = notConsistentMsg
			},
		};

		[TestCaseSource(nameof(notConsistentindents))]
		public void On_NotConsistentindents(CodeTestCase testCase)
		{
			indentsValidator.ReportWarningIfIndentsNotValidated(testCase.Code);
			validatorOut.ToString().Should().Be(testCase.Warning);
		}
	}
}