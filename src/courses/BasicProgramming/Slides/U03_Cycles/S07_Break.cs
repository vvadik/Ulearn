using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Break", "{43E238F8-424F-40D5-95AF-CF1575EF630D}")]
	class S07_Break
	{
		/*
		##Задача: Break
		
		Вас снова просят сделать что-то очень странное. Вам необходимо прибавлять к строке нужный символ, пока строка не станет длины 10,
		либо пока не выполнится условие странной функции MagicChecker от текущего состояния строки, которую вам дал заказчик.
		После чего, вы опять же должны вызвать еще одну странную функцию MagicFunction от итоговой строки, после чего вернуть её
		*/

		[ExpectedOutput("82\r\n70\r\n70\r\nstartc\r\nkittyyyyyy\r\nC+++++++++")]
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
		public static string Start(string startstring, string s)
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

		public static void MagicFunction(string str)
		{
			Console.WriteLine((str.Length*397)%100);
		}

		public static bool MagicChecker(string str)
		{
			return str.Substring(str.Length / 2).Last() % 3 == 0;
		}
	}
}
