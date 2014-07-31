using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("If return", "{A72B8529-FF6D-4C57-9DB7-19999AC4A3B2}")]
	internal class S04_IfReturn
	{
		/*
		##Задача: if return
		
		Вася продолжает апгрейдить свой комп и выбирает себе мышку.
		Для этого он бросает 16-ти гранный кубик и, в зависимости от результата, выбирает мышку.

		 * если выпало число от 1 до 5, он выбирает мышку FunnyMouse
		 * если выпало число от 6 до 8, он выбирает мышку LuckyMouse
		 * если выпало число от 9 до 16, он выбирает мышку CSharpMouse
		
		Напишите функцию, которая по значению числа на кубике возвращает соответствующую мышку. Постарайтесь написать как можно меньше кода.
		*/

		public enum Mouses
		{
			CSharpMouse,
			LuckyMouse,
			FunnyMouse
		}

		[Hint("переменная типа bool, приведенная к строке, начинается с заглавной буквы")]
		[ExpectedOutput("CSharpMouse\r\nFunnyMouse\r\nLuckyMouse\r\nCSharpMouse")]
		[ShowOnSlide]
		public static void Main()
		{
			Mouses firstMouse = GetMouse(13);
			Mouses secondMouse = GetMouse(1);
			Mouses thirdMouse = GetMouse(7);
			Mouses fourthMouse = GetMouse(9);
			Console.WriteLine(firstMouse.ToString());
			Console.WriteLine(secondMouse.ToString());
			Console.WriteLine(thirdMouse.ToString());
			Console.WriteLine(fourthMouse.ToString());
		}

		[Exercise]
		private static Mouses GetMouse(int diceNumber)
		{
			if (diceNumber <= 5) return Mouses.FunnyMouse;
			return diceNumber >= 9 ? Mouses.CSharpMouse : Mouses.LuckyMouse;
			/*uncomment
			...
			*/
		}
	}
}
