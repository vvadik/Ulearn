using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	[Slide("Подсчет", "{9EB1A5C2-135D-49A5-A922-0F3F91566080}")]
	class S017_CountElement
	{
		/*
		Тренировки продолжаются. На этот раз вы с Васей на перегонки решаете одну задачу — найти количество вхождений заданного числа в массив чисел.

		На старт! Внимание! Марш!
		*/

		[ExpectedOutput(@"
1
0
1
2
0
4
")]
		[HideOnSlide]
		public static void Main()
		{
			Console.WriteLine(GetElementCount(new [] { 1, 2, 3 }, 1));
			Console.WriteLine(GetElementCount(new int[] { }, 42));
			Console.WriteLine(GetElementCount(new [] { 1 }, 1));
			Console.WriteLine(GetElementCount(new [] { 0, 100, 1, 2, 100 }, 100));
			Console.WriteLine(GetElementCount(new [] { 1, 2, 3, 100, 4, 5, 6 }, 42));
			Console.WriteLine(GetElementCount(new [] { 100, 100, 100, 100 }, 100));
		}

		[Exercise]
		[Hint("Используйте foreach в связке с if. Введите вспомогательную переменную для хранения количества уже найденных вхождений itemToFind")]
		public static int GetElementCount(int[] items, int itemToFind)
		{
			var count = 0;
			foreach (var item in items)
				if (item == itemToFind) count++;
			return count;
		}
	}
}
