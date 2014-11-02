using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Исправить рекурсию", "A5BA07A0-3B06-424B-989A-330137C1C05E")]
	public class S015_FixRecursion : SlideTestBase
	{
		/*
		Вася решил, что изучать рекурсию нужно на простых примерах
		и начал c программы, печатающей все элементы массива в обратном порядке.

		Как это сделать рекурсивно? Очень просто: сначала решить задачу, для всего массива, кроме первого элемента, а потом вывести первый элемент.

		Идея проста, но в реализации что-то пошло не так. Видимо, Вася упустил какую-то важную деталь.
		
		Найдите ошибку Васи и помогите ему исправить программу. Естественно, программа должна остаться рекурсивной, ведь именно в этом смысл упражнения!
		*/

		[ExpectedOutput(@"
WriteReversed(new char[]{ '1', '2', '3' })
321
WriteReversed(new char[]{ '1', '2' })
21
WriteReversed(new char[]{ '1' })
1
WriteReversed(new char[]{  })

WriteReversed(new char[]{ '1', '1', '2', '2', '3', '3' })
332211
WriteReversed(new char[]{ '1', '2', '3', '4' })
4321
WriteReversed(new char[]{ 'a', 'b', 'c', 'd' })
dcba
")]
		[HideOnSlide]
		public static void Main()
		{
			RunTest("123");
			RunTest("12");
			RunTest("1");
			RunTest("");
			RunTest("112233");
			RunTest("1234");
			RunTest("abcd");
		}

		[HideOnSlide]
		private static void RunTest(string input)
		{
			Console.WriteLine("WriteReversed(new char[]{{ {0} }})", string.Join(", ", input.ToCharArray().Select(c => "'" + c + "'").ToArray()));
			WriteReversed(input.ToCharArray());
			Console.WriteLine();
		}

		[Exercise]
		public static void WriteReversed(char[] items, int startIndex = 0)
		{
			if (startIndex >= items.Length)
				return;
			WriteReversed(items, startIndex + 1);
			Console.Write(items[startIndex]);
			/*uncomment
			// Выводим в обратном порядке все элементы с индексом больше startIndex
			WriteReversed(items, startIndex+1); 
			// а потом выводим сам элемент startIndex
			Console.Write(items[startIndex]); 
			*/
		}
	}
}