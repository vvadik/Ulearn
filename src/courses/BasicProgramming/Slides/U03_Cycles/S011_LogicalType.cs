using System;

namespace uLearn.Courses.BasicProgramming.Slidess
{
	[Slide("Сравнение и логический тип 2", "{C56C7CDE-64BD-4023-9D6D-6328E3302C00}")]
	class S011_LogicalType
	{
		/*
		##Задача: Логические выражения и мечты
		
		К сожалению, вы попали не в Хогвартс. Но и в мире информатики полно магии.
		Например, некоторые программисты, как например Вася, пишут магические функции. А потом не могут сделать так, чтобы они давали нужный результат.
		Вам же осталось лишь подобрать переменные a, b, c, d с помощью нереализованных функций так, чтобы магическия функция Magic выдавала желаемый результат.
		*/
		[Hint("переменная типа bool, приведенная к строке, начинается с заглавной буквы")]
		[ExpectedOutput("False\r\nTrue\r\nFalse")]
		public static void Main()
		{
			var a = GetA();
			var b = GetB();
			var c = GetS();
			var d = GetD();
			MagicFunction(a, b, c, d); // Should print: False, True, False
		}

		public static void MagicFunction(int a, int b, string c, bool d)
		{
			var abrakadabra = a > b;
			var patronus = d.ToString() == "False";
			var avadaKedavra = (c == "true") == d;
			Console.WriteLine(abrakadabra == patronus);
			Console.WriteLine(avadaKedavra == abrakadabra);
			Console.WriteLine(avadaKedavra == patronus);
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static string GetS()
		{
			return "true";
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static int GetB()
		{
			return 4;
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static bool GetD()
		{
			return false;
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static int GetA()
		{
			return 2;
		}


	}
}
