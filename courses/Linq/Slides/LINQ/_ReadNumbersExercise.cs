using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.Linq.Slides.LINQ.LINQ.LINQ
{
	public class S020_ReadNumbersExercise : SlideTestBase
	{
		/*
		`LINQ` удобно использовать для чтения из файла и разбора простых текстовых форматов.
		Особенно удобно сочетать методы `LINQ` с методами класса `File`: 
		`File.ReadLines(filename)`, `File.WriteLines(filename, lines)`.

		На выходе метода ReadAllLines получается массив строк, поэтому часто возникает задача обработки именно массива строк.
		
		Пусть у вас есть массив строк, в котором каждая строка либо пустая, либо содержит одно целое число.
		Напишите метод, который вернет массив всех чисел, присутствующих в исходном списке строк.

		Ваш код будет проверяться с помощью такого метода `Main`:
		*/

		public static void Main()
		{
			foreach (var num in ParseNumbers(new[] {"-0", "+0000"}))
				Console.WriteLine(num);
			foreach (var num in ParseNumbers(new List<string> {"1", "", "-03", "0"}))
				Console.WriteLine(num);
		}

		/*
		Реализуйте метод `ParseNumbers` в одно `LINQ`-выражение.
		*/

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