using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Создание методов расширения", "{06AA4E3E-C1F8-4895-BA1F-B7D5DF22BB28}")]
	public class S045_CreateExtension : SlideTestBase
	{
		/*
		И снова сделайте так, чтобы код заработал!
		*/

		[ExpectedOutput("100542")]
		[Hint("Создайте метод расширения класса String, преобразующий строку в int.")]
		[Hint("Метод расширения должен быть определен в статическом классе, а перед его первым аргументом должно быть ключевое слово this")]
		public static void Main()
		{
			var arg1 = "100500";
			Console.Write(arg1.ToInt() + "42".ToInt()); // 100542
		}

	}
	
	[Exercise]
	public static class StringExtensions
	{
		public static int ToInt(this string text)
		{
			return Int32.Parse(text);
		}
	}
}