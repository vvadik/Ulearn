using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace uLearn.Courses.BasicProgramming.Slides
{
	class S07_Break
	{
		/*
		##Задача: Break
		Вас снова просят сделать что-то очень странное. Вам необходимо прибавлять к строке нужный символ, пока строка не станет длины 10,
		либо пока не выполнится условие странной функции MagicChecker от текущего состояния строки, которую вам дал заказчик.
		После чего, вы опять же должны вызвать еще одну странную функцию MagicFunction от итоговой строки, после чего вернуть её
		*/

		[Hint("у каждого объекта в C# есть функция GetHashCode, которая однозначно определяет специальный код этого объета.\n" +
		      "Это необходимо для сравнения объектов в некоторых случаях, о них вы узнаете позднее.")]
		[ExpectedOutput("43\r\n-42\r\n-53\r\nstartccccc\r\nkittyyyy\r\nC++++++")]
		[ShowOnSlide]
		public static void Main()
		{
			string string1 = Start("start", "c");
			string string2 = Start("kitty", "y");
			string string3 = Start("C++", "+");
			Console.WriteLine(string1);
			Console.WriteLine(string2);
			Console.WriteLine(string3);
		}

		[Exercise]
		private static string Start(string startstring, string s)
		{
			while (true)
			{
				if (startstring.Length >= 10)
					break;
				if (MagicChecker(startstring))
					break;
				startstring += s;
			}
			MagicFunction(startstring);
			return startstring;
			/*uncomment
			...
			MagicFunction(startstring);
			return startstring;
			*/
		}

		[ShowOnSlide]
		private static void MagicFunction(string str)
		{
			Console.WriteLine(str.GetHashCode()%100);
		}

		[ShowOnSlide]
		private static bool MagicChecker(string startstring)
		{
			return startstring.GetHashCode() < 0 && startstring.GetHashCode()%3 == 0;
		}
	}
}
