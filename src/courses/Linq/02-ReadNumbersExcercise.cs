using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SharpLessons.Linqed
{
	[TestFixture]
	public class ReadNumbersExcercise
	{
		/*

		##Задача: Чтение из файла

		Linq удобно использовать для чтения из файла и разбора простых текстовых формат. Особенно удобно сочетать Linq с методом `File.ReadLines(filename)`.

		Предположим в файле filename в каждая строка либо пустая, либо содержит одно целое число. 
		Нужно прочитать числа из файла в массив. Решите задачу в один LINQ-запрос.

		###Пример файла
		    1
		    
		    -3
		    0

		Решение этой задачи будет использоваться следующим образом:
		*/

		[Sample]
		public void ParseNumber_Sample()
		{
			int[] numbers = ParseNumbers(File.ReadLines("numbers.txt"));
		}

		[Exercise(SingleStatement = true)]
		[Hint("`int.Parse` преобразует строку в целое число.")]
		public int[] ParseNumbers(IEnumerable<string> lines)
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

		[Test]
		public void Test()
		{
			int[] actualNumbers = ParseNumbers(new[] {"", "1", "2", "", "42"});
			Assert.That(
				actualNumbers,
				Is.EqualTo(new[] {1, 2, 42}).AsCollection);
		}

		/*
		### Краткая справка
		  * `items.Where(item => isGoodItem(item))` — фильтрация последовательности
		  * `items.Select(item => convert(item))` — поэлементное преобразование последовательности
		  * `items.ToArray()` — преобразование последовательности в массив
		*/
	}
}