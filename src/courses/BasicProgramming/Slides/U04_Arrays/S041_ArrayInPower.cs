using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Помогите Васе написать метод, который принимает массив ```int[]```,
	возводит все его элементы в заданную степень и возвращает массив с результатом этой операции.

	Исходный массив при этом должен остаться неизменным.
	*/

	[Slide("Возвести массив в степень", "{E3E45EC7-7BD0-4284-8CA1-0FBCB2FA0C21}")]
	public class S041_ArrayInPower : SlideTestBase
	{
		[ExpectedOutput("1, 2, 3, 4, 5, 6, 7, 8, 9\n1, 4, 9, 16, 25, 36, 49, 64, 81\n1, 8, 27, 64, 125, 216, 343, 512, 729\n\n1")]
		public static void Main()
		{
			var arrayToPower = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
			PrintArray(GetPoweredArray(arrayToPower, 1));
			PrintArray(GetPoweredArray(arrayToPower, 2));
			PrintArray(GetPoweredArray(arrayToPower, 3));
			PrintArray(GetPoweredArray(new int[0], 1));
			PrintArray(GetPoweredArray(new []{42}, 0));
		}

		[Exercise]
		[Hint("Вспомните, что такое ссылочные типы. Массив — это ссылочный тип!")]
		[Hint("Вам нужно создать новый массив, а не менять переданный вам.")]
		public static int[] GetPoweredArray(int[] arr, int power)
		{
			return arr.Select(x => (int)Math.Pow(x, power)).ToArray();
		}

		[HideOnSlide]
		public static void PrintArray(int[] a)
		{
			Console.WriteLine(string.Join(", ",a.Select(x => x.ToString()).ToArray()));
		}
	}
}
