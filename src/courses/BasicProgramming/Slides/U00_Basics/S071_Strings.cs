using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U00_Basics
{
	[Slide("Ваш первый метод", "{d8578c7f-d7e2-4ce5-a3be-e6e32d9ac63e}")]
	class S071_Strings : SlideTestBase
	{
		/*
		Напишите тело метода так, чтобы он возвращал вторую половину строки `text`. Можете считать, что `text` всегда четной длины.

		Всю информацию о доступных методах класса `String` вы можете прочитать в [официальной документации .NET](http://msdn.microsoft.com/ru-ru/library/system.string(v=vs.110).aspx)
		*/
		[ExpectedOutput("CSharp!\n67890\n соль ля си")]
		static void Main()
		{
			Console.WriteLine(GetLastHalf("I love CSharp!"));
			Console.WriteLine(GetLastHalf("1234567890"));
			Console.WriteLine(GetLastHalf("до ре ми фа соль ля си"));
		}

		[Exercise]
		[Hint("Можете ли вы собрать решение из известных вам уже функций работы со строками?")]
		static string GetLastHalf(string text)
		{
			return text.Substring(text.Length / 2);
			/*uncomment
			...
			*/
		}
	}
}
