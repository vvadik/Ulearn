using uLearn;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Статические классы", "BD862F02-7CF0-4582-9861-189CF89CF22B")]
	public class S060_StaticClass
	{
		//#video _NIB9Amr1qc
		/*
		## Заметки по лекции
		*/
		static class StaticAlgorithm
		{
			static int temporalValue;
			static public int Run(int value)
			{
				var result = 0;
				for (temporalValue = 0; temporalValue <= value; temporalValue++)
					result += temporalValue;
				return result;
			}
		}

		class Algorithm
		{
			int temporalValue;

			public int Run(int value)
			{
				var result = 0;
				for (temporalValue = 0; temporalValue <= value; temporalValue++)
					result += temporalValue;
				return result;
			}
		}

		/*
		Сравнение вызова статического алгоритма и алгоритма, оформленного в виде класса.
		*/
		public static void Main()
		{
			StaticAlgorithm.Run(10);

			var algorithm = new Algorithm();
			algorithm.Run(10);
		}
	}
}