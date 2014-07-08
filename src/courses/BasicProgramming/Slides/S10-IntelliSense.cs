using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	public class S10_IntelliSense
	{
		/*

		##Задача: Добрый работодатель
		
		Напишите метод, которая реализует следующие требования:
		метод принимает на вход имя(Name) и зарплату(N). Должна напечатать строку следующего вида:
		"Hello, Name, you salary is N". Но так как работадатель очень добр, он всегда округляет дробная зарплату до ближайшего целого числа вверх.
		*/

		[Exercise(SingleStatement = true)]
		[Hint("IntelliSense - сила")]
		[ExpectedOutput("Hello, Kitty, your salary is 101")]
		public static void MainX()
		{
			PrintGreeting("Kitty", 100.01);
			/*uncomment
			PrintGreeting("Kitty", 100.01);
			*/
		}

		private static void PrintGreeting(string name, double salary)
		{
			var ans = new StringBuilder("Hello, *, your salary is ");
			ans.Replace("*", name);
			ans.Append(Math.Ceiling(salary));
			Console.WriteLine(ans.ToString());
		}

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("MainX"));
		}
	}
}
