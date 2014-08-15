using System;
using System.Text;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Добрый работодатель", "{F6559650-B3AF-4E5E-BE84-941FB21BC2AC}")]
	public class S100_FinalTask
	{
		/*

		Вася до завтра должен написать важную подпрограмму для Доброго Работодателя.
		Оставалось дописать всего один метод, когда Вася от переутомления крепчайше заснул.

		Напишите метод, который принимает на вход имя <Name> и зарплату <Salary> и возвращает строку вида:
		```Hello, <Name>, your salary is <Salary>```. 
		
		Но так как Работадатель Добр, он всегда округляет зарплату до ближайшего целого числа вверх.

		Во многих редакторах и IDE сочетание клавиш ```Ctrl + Space``` показывает контекстную подсказку.
		Тут подсказки также работают, однако внутри Visual Studio они гораздо полнее и удобнее.
		*/

		[Hint("В .NET уже есть функция округления числа вверх. Попробуйте ее найти самостоятельно.")]
		[Hint("Функция округления вверх находится в классе Math")]
		[Hint("Функция округления вверх — Math.Ceiling")]
		[ExpectedOutput(@"Hello, Student, your salary is 11
Hello, Bill Gates, your salary is 10000001
Hello, Steve Jobs, your salary is 1")]
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
