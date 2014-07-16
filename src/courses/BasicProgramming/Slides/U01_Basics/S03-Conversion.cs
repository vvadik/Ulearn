using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Преобразования типов")]
	public class S03_Conversion
	{
		/*
		Исправьте все ошибки преобразования типов.
		*/

		[Exercise]
		[ExpectedOutput("31415\n31416")]
		public static void Main()
		{
			double pi = Math.PI;
			int tenThousend = (int)10000L;
			float floatPi = (float) pi;
			float tenThousendPi = floatPi*tenThousend;
			int roundedTenThousendPi = (int)Math.Round(tenThousendPi);
			int integerPartOfTenThousednPi = (int)tenThousendPi;
			Console.WriteLine(integerPartOfTenThousednPi);
			Console.WriteLine(roundedTenThousendPi);
			/*uncomment
			double pi = Math.PI;
			int tenThousend = 10000L;
			float floatPi = pi;
			float tenThousendPi = floatPi*tenThousend;
			int roundedTenThousendPi = tenThousendPi;
			int integerPartOfTenThousednPi = tenThousendPi;
			Console.WriteLine(integerPartOfTenThousednPi);
			Console.WriteLine(roundedTenThousendPi);
			*/
		}
	}
}
