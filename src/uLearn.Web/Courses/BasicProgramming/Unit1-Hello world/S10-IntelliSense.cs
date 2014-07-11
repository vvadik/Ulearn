using System;
using System.Text;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Автокомплит")]
	public class S10_IntelliSense
	{
		/*

		##Задача: Добрый работодатель
		
		Напишите метод, которая реализует следующие требования:
		метод принимает на вход имя(Name) и зарплату(N). Должна напечатать строку следующего вида:
		"Hello, Name, you salary is N". Но так как работадатель очень добр, он всегда округляет дробная зарплату до ближайшего целого числа вверх.
		*/

		[Hint("IntelliSense - сила")]
		[ExpectedOutput("Hello, Kitty, your salary is 101\r\nHello, World, your salary is 0")]
		[ShowOnSlide]
		public static void Main()
		{
			Console.WriteLine(PrintGreeting("Kitty", 100.01));
			Console.WriteLine(PrintGreeting("World", 0));
		}

		[Exercise]
		private static string PrintGreeting(string name, double salary)
		{
			var ans = new StringBuilder("Hello, *, your salary is ");
			ans.Replace("*", name);
			ans.Append(Math.Ceiling(salary));
			return ans.ToString();
		}

	}
}
