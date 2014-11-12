using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Создание классов", "{06AA4E3E-C1F8-4895-BA1F-B7D5DF22BB28}")]
	public class S045_CreateExtension : SlideTestBase
	{
		/*
		Сделайте так, чтобы заработало. 
		*/

		[ExpectedOutput("15")]
		[Hint("Создайте extension-метод, который бы присоединялся к типу массива int, и вычислял бы сумму элементов этого массива.")]
		[Hint("Extension методы — статические, определяются в статических классах, имеют первый аргумент с ключевым словом this")]
		static void Main()
		{
			var array = new int[] { 1, 2, 3, 4, 5 };
			Console.Write(array.Sum());
		}

	}
	
	[HideOnSlide]
	[ExcludeFromSolution]
	public static class ArrayExtensionHidden
	{
		[Exercise]
		public static int Sum(this int[] array)
		{
			var sum = 0;
			foreach (var e in array)
				sum += e;
			return sum;
		}
	}
}