using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class LineLengthChecker_should
	{
		[TestCase("protected override void getX(ref int x) {}")]
		[TestCase("01234567891")]
		[TestCase("										1")]
		public void Warn_LongLines(string code)
		{
			CheckIncorrect(code, "длинная");
		}
		[TestCase("0123456789")]
		[TestCase("int a=142;")]
		[TestCase("								1;")]
		public void Pass_ShortLines(string code)
		{
			CheckCorrect(code);
		}

		private static void CheckCorrect(string code)
		{
			Assert.That(FindErrors(code), Is.Null, code);
		}
		private void CheckIncorrect(string incorrectCode, string messageSubstring)
		{
			Assert.That(FindErrors(incorrectCode), Is.StringContaining("Строка 1").And.StringContaining(messageSubstring), incorrectCode);
		}

		private static string FindErrors(string code)
		{
			return new LineLengthStyleValidator(10).FindError(code);
		}
	}
}