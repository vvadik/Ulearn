using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U14_Structures
{
	[Slide("Применение ref", "3645CCF5FA0A4DAFAD5356A58C8827B1")]
	class S045_Сделать_ref : SlideTestBase
	{
		/*
		В этой задаче демонстрируется один из редких, когда уместно использовать `ref`.

		`SkipSpaces` пропускает все пробельные символы в `text`, начиная с позиции `pos`. 
		В конце `pos` оказывается установлен в первый непробельный символ.
		*/

		public static void SkipSpaces(string text, ref int pos)
		{
			while (pos < text.Length && char.IsWhiteSpace(text[pos]))
				pos++;
		}
		
		/*
		`ReadNumber` пропускает все цифры в `text`, начиная с позиции `pos`, а затем возвращает все пропущенные символы.
		В конце `pos` оказывается установлен в первый символ не цифру.
		*/

		public static string ReadNumber(string text, ref int pos)
		{
			var start = pos;
			while (pos < text.Length && char.IsDigit(text[pos]))
				pos++;
			return text.Substring(start, pos - start);
		}

		/*
		Допишите функцию, которая читает все числа из строки и выводит их, разделяя единственным пробелом.
		*/

		[Exercise]
		public static void WriteAllNumbersFromText(string text)
		{
			var pos = 0;
			while (true)
			{
				SkipSpaces(text, ref pos);
				var num = ReadNumber(text, ref pos);
				if (num == "") break;
				Console.Write(num + " ");
			}
			Console.WriteLine();
			/*uncomment
			while (true)
			{
				// skip spaces
				var num = // read next number
				if (num == "") break;
				Console.Write(num + " ");
			}
			Console.WriteLine();

			*/
		}

		[ExpectedOutput(@"1 2 3 
4 
567890 

10 20 30 ")]
		[HideOnSlide]
		public static void Main()
		{
			WriteAllNumbersFromText("    1  2      3");
			WriteAllNumbersFromText("4");
			WriteAllNumbersFromText("567890");
			WriteAllNumbersFromText("");
			WriteAllNumbersFromText("10 20 30 ");
		}
	}
}
