using NUnit.Framework;
using uLearn.CSharp.Validators;

namespace uLearn.CSharp.RedundantElseValidation
{
	[TestFixture]
	public class RedundantElseValidator_Should
	{
		private readonly RedundantElseValidator validator = new RedundantElseValidator();

		[Test]
		public void Test()
		{
			var code = @"
bool Function()
{
	if (true)
	{
		if (true)
		return true;
	else
		return false;
	}
	else
		return false;
}";
			validator.FindErrors(code);
		}
	}
}