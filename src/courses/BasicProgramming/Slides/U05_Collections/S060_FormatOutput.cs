using System;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	//#video EXenLjobvwc
	/*
	## Заметки по лекции
	*/
	[Slide("Форматированный вывод", "{458FFA94-D602-4E54-857D-FCEAF5E505FA}")]
	public class S060_FormatOutput
	{
		[Test]
		public static void Main()
		{

			var a = 13;
			var b = 14.3456789;

			//Всегда можно писать так:
			Console.WriteLine(a + " " + b);

			//Но для больших документов это не удобно. Кроме того, не получится настроить, 
			//например, количество цифр после запятой
			//Используйте форматированный вывод
			Console.WriteLine("{0} {1}", a, b);

			//Для того, чтобы отформатировать строку без вывода, используйте string.Format
			//На самом деле, Console.WriteLine просто вызывает string.Format.
			var formattedString = string.Format("{0} {1}", a, b);

			//Форматированный вывод позволяет настроить точность округления
			Console.WriteLine("{0:0.000} {1:0.0000}", 1.23456, 1.23456);

			//Вывод завершающих нулей
			Console.WriteLine("{0:0.000} {1:0.###}", 1.2, 1.2);

			//Добивание нулями слева
			Console.WriteLine("{0:D4}", 42); //0042

			//Разбиение на колонки и выравнивание по левому
			Console.WriteLine("{0,10}|\n{1,10}|", 12345, 123);

			//или правому краю
			Console.WriteLine("{0,-10}|\n{1,-10}|", 12345, 123);

			//А также комбинации выравнивания и округления
			Console.WriteLine("{0,10:0.00}|\n{1,10:0.000}|", 1.45, 21.345);

			//Форматирование времени и даты
			Console.WriteLine("{0:hh:mm:ss}", DateTime.Now);

			// MM и mm — это Месяцы и минуты. Различаются только регистром.
			Console.WriteLine("{0:yy-MM-dd}", DateTime.Now);

			// Можно менять количество букв и порядок:
			Console.WriteLine("{0:d-MM-yyyy}", DateTime.Now); //1-12-2014

			//Фигурные скобки НЕ ЯВЛЯЮТСЯ спецсимволами шарпа:
			Console.WriteLine("{}"); //Это будет работать!

			//Но они являются спецсимволами метода string.Format, и их нельзя использовать просто так,
			//если вызывается этот метод
			//Console.WriteLine("{0}{}", a); //Это скомпилируется, но выбросит исключение

			//Надо их экранировать удвоением
			Console.WriteLine("{0}{{}}", a);
		}

		/*
		[Документация по форматированию числовых значений](http://msdn.microsoft.com/ru-ru/library/0c899ak8(v=vs.110).aspx)
		*/
	}
}