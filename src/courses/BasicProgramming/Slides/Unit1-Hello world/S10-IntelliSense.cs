using System;
using System.Text;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Intellisence")]
	public class S10_IntelliSense
	{
		/*

		##Задача: Добрый работодатель
		
		Напишите метод, который реализует следующие требования:
		Метод принимает на вход имя (Name) и зарплату (N). Метод должен напечатать строку следующего вида:
		"Hello, (Name), you salary is (N)". Но, так как работадатель очень добр, он всегда округляет дробную зарплату до ближайшего целого числа вверх.
		*/

		[Hint("IntelliSense - сила. Для более удобного и понятного intellisence рекомендуется использовать visual studio. Для того, чтобы пользоваться intellisence на нашем сайте, нажмите Ctrl + space.")]
		[ExpectedOutput("Hello, Kitty, your salary is 101\r\nHello, Jack, your salary is 5")]
		[ShowOnSlide]
		public static void Main()
		{
			Console.WriteLine(PrintGreeting("Kitty", 100.01));
			Console.WriteLine(PrintGreeting("Jack", 4.5));
		}

		[Exercise]
		private static string PrintGreeting(string name, double salary)
		{
			var ans = new StringBuilder("Hello, *, your salary is ");
			ans.Replace("*", name);
			ans.Append(Math.Ceiling(salary));
			return ans.ToString();
			/*uncomment
			string template = "Hello, *, your salary is ";
			...
			*/
		}

	}
}
