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
		public static string[] correctFilenames => IndentsTestHelper.CorrectTestDataDir.GetFiles().Select(f => f.Name)
			.ToArray();

		[TestCaseSource(nameof(correctFilenames))]
		public void On_CorrectIndents(string filename)
		{
			var code = IndentsTestHelper.CorrectTestDataDir.GetFiles(filename).Single().ContentAsUtf8();
			var validator = new IndentsValidator(code);
			var validatorOut = new StringBuilder();
			validator.Warning += msg => validatorOut.Append(msg);

			validator.ReportFirstWarningIfIndentsNotValidated();

			validatorOut.ToString().Should().BeEmpty();
		}
	}
}