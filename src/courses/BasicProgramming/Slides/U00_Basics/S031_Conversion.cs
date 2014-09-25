using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U00_Basics
{
	[Slide("Ошибки преобразования типов", "{04D61C06-8A32-41A6-A47E-2AE2D095DEF3}")]
	public class S031_Conversion : SlideTestBase
	{
		/*
		Исправьте все ошибки преобразования типов.
		*/

		[Exercise]
		[ExpectedOutput("31415\n31416")]
		[Hint("Вспомните о том, что в С# есть приведение типов ")]
		[Hint("Округление можно найти в библиотеке Math")]
		public static void Main()
		{
			double pi = Math.PI;
			int tenThousand = (int)10000L;
			float floatPi = (float) pi;
			float tenThousandPi = floatPi * tenThousand;
			int roundedTenThousandPi = (int)Math.Round(tenThousandPi);
			int integerPartOfTenThousadnPi = (int)tenThousandPi;
			Console.WriteLine(integerPartOfTenThousadnPi);
			Console.WriteLine(roundedTenThousandPi);
			/*uncomment
			double pi = Math.PI;
			int tenThousand = 10000L;
			float floatPi = pi;
			float tenThousandPi = floatPi*tenThousand;
			int roundedTenThousandPi = tenThousandPi;
			int integerPartOfTenThousandPi = tenThousandPi;
			Console.WriteLine(integerPartOfTenThousandPi);
			Console.WriteLine(roundedTenThousandPi);
			*/
		}
	}
}
