using System;
using System.Collections.Generic;
using System.Linq;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Чтение списка чисел", "{CBA7BC68-F1B9-46B1-93D4-49AC113A1D02}")]
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

		Допустим, что кто-то уже вызвал метод `File.ReadLines(filename)` и теперь у вас есть массив всех строк файла.
		Реализуйте метод ParseNumbers.
		*/


		[ExpectedOutput("0\n0\n1\n-3\n0")]
		public static void Main()
		{
			foreach (var num in ParseNumbers(new[] {"-0", "+0000"}))
				Console.WriteLine(num);
			foreach (var num in ParseNumbers(new List<string> {"1", "", "-03", "0"}))
				Console.WriteLine(num);
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
	}
}