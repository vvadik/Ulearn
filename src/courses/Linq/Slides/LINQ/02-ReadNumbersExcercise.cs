using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Id("Read_Number_Ex")]
	[Title("Задача. Чтение списка чисел")]
	[TestFixture]
	public class ReadNumbersExcercise
	{
		/*

		Linq удобно использовать для чтения из файла и разбора простых текстовых формат. Особенно удобно сочетать Linq с методом `File.ReadLines(filename)`.

		Предположим в файле filename в каждая строка либо пустая, либо содержит одно целое число. 
		Нужно прочитать числа из файла в массив. Решите задачу в один LINQ-запрос.

		###Пример файла
		    1
		    
		    -3
		    0

		Допустим, что у нас уже есть массив строк, полученный из файла, реализуйте метод ParseNumbers
		*/

		[ShowOnSlide]
		[ExpectedOutput("1\r\n2\r\n3")]
		public static void Main()
		{
			string[] strNumbers = new string[] {"1", "2", "3"};
			int[] parsedNumbers = ParseNumbers(strNumbers);
			foreach (var e in parsedNumbers)
				Console.WriteLine(e);
		}

		[Exercise]
		[Hint("`int.Parse` преобразует строку в целое число.")]
		public static int[] ParseNumbers(IEnumerable<string> lines)
				{
					return lines
						.Where(line => line != "")
						.Select(int.Parse)
						.ToArray();
					/*uncomment
					return lines
						.Where(...)
						.Select(...)
						...
					*/
				}

		/*
		### Краткая справка
		  * `items.Where(item => isGoodItem(item))` — фильтрация последовательности
		  * `items.Select(item => convert(item))` — поэлементное преобразование последовательности
		  * `items.ToArray()` — преобразование последовательности в массив
		*/
	}
}