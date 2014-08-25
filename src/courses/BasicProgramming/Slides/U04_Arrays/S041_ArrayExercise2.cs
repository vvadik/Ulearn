using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Помогите Васе написать
	метод, который принимает массив ```int[]``` и возводит все его элементы в нужную степень,
 	возвращая результирующий массив. Исходный массив должен остаться неизменным.
	*/

	[Slide("Степени", "{E3E45EC7-7BD0-4284-8CA1-0FBCB2FA0C21}")]
	static class S041_ArrayExercise2
	{
		[ExpectedOutput("1, 2, 3, 4, 5, 6, 7, 8, 9\n1, 4, 9, 16, 25, 36, 49, 64, 81\n1, 8, 27, 64, 125, 216, 343, 512, 729")]
		public static void Main()
		{
			var integersToIncrease = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
			PrintArray(GetIncreasedArray(integersToIncrease, 1));
			PrintArray(GetIncreasedArray(integersToIncrease, 2));
			PrintArray(GetIncreasedArray(integersToIncrease, 3));
		}

		[Exercise]
		[Hint("Вспомните, что такое ссылочные типы")]
		[Hint("Массив - ссылочный тип")]
		public static int[] GetIncreasedArray(int[] arr, int degree)
		{
			return arr.Select(x => (int)Math.Pow(x, degree)).ToArray();
			/*uncomment
			 ...
			*/
		}

		[HideOnSlide]
		public static void PrintArray(int[] a)
		{
			Console.WriteLine(string.Join(", ",a.Select(x => x.ToString()).ToArray()));
		}
	}
}
