using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class IndentsValidator_Should_NotReport
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

		public static string[] correctFilenames => IndentsTestHelper.CorrectTestDataDir.GetFiles().Select(f => f.Name).ToArray();

		[TestCaseSource(nameof(correctFilenames))]
		public void On_CorrectIndents(string filename)
		{
			var code = IndentsTestHelper.CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			indentsValidator.ReportWarningIfIndentsNotValidated(code);
			validatorOut.ToString().Should().BeEmpty();
		}
	}
}