using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Лишний if", "{FA865454-05CB-4297-ADA7-4D65D32A3F4F}")]
	class S05_ExcessIf
	{
		/*
		##Задача: Excess if
		
		Вася уже успел стать крутым программистом и попал на чемпионат по самому короткому коду.
		Пмогите ему выиграть, реализовав нужный метод всего в одну строчку, и, может быть, с вами поделятся плюшками.

		Метод должен возвращать значение, противоположное дизъюнкции содержания в строке первой буквы "m" и последней буквы "c",
		с тем, что две переменные, переданные в конце, не равны.
		*/

		[Hint("Пользуйтесь автокомплитом для поиска методов у строк, и не только.")]
		[ExpectedOutput("False\r\nTrue\r\nFalse\r\nTrue")]
		public static void Main()
		{
			Console.WriteLine(ShortFunction("magic", true, false));
			Console.WriteLine(ShortFunction("magic", true, true));
			Console.WriteLine(ShortFunction("mage", false, true));
			Console.WriteLine(ShortFunction("CSharp", false, false));
		}

		[Exercise]
		public static bool ShortFunction(string str, bool firstBoolean, bool secondBoolean)
		{
			return !((str.StartsWith("w") && str.EndsWith("c")) || (firstBoolean != secondBoolean));
			/*uncomment
			return ...
			*/
		}
	}
}
