using System;
using System.Text;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Добрый работодатель")]
	public class S10_IntelliSense
	{
		/*

		Вася до завтра должен написать важную подпрограмму для Доброго Работодателя.
		Осталось дописать всего один метод, когда Вася от переутомления крепчайше заснул.

		Напишите метод, который на вход имя (name) и зарплату (salary) и возвращает строку вида:
		```Hello, (Name), you salary is (N)```. 
		
		Но так как Работадатель Добр, он всегда округляет зарплату до ближайшего целого числа вверх.

		Во многих редакторах и IDE сочетание клавиш ```Ctrl + Space``` показывает контекстную подсказку.
		Тут подсказки также работают, однако внутри Visual Studio они гораздо полнее и удобнее.
		*/

		[Hint("В .NET уже есть функция округления числа вверх. Попробуйте ее найти самостоятельно.")]
		[Hint("Функция округления вверх находится в классе Math")]
		[Hint("Функция округления вверх — Math.Ceiling")]
		[ExpectedOutput(@"Hello, Student, your salary is 11
Hello, Bill Gates, your salary is 10000001
Hello, Steve Jobs, your salary is 1
")]
		[ShowOnSlide]
		public static void Main()
		{
			
			Console.WriteLine(PrintGreeting("Student", 10.01));
			Console.WriteLine(PrintGreeting("Bill Gates", 10000000.5));
			Console.WriteLine(PrintGreeting("Steve Jobs", 1));
		}

		[Exercise]
		private static string PrintGreeting(string name, double salary)
		{
			var ans = new StringBuilder("Hello, *, your salary is ");
			ans.Replace("*", name);
			ans.Append(Math.Ceiling(salary));
			return ans.ToString();
			/*uncomment
			// возвращает "Hello, <name>, your salary is <salary>"
			...
			*/
		}

	}
}
