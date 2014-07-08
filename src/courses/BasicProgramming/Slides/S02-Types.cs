using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	public class S02_Types
	{
		/*
		##Задача: Соответствие типов данных
		Неправильно указанный тип данных - частая ошибка в работе программиста, которую он совершил и сейчас. Исправьте положение!
		*/
		[Exercise(SingleStatement = true)]
		[ExpectedOutput("5.5\r\n7.8\r\n0")]
		static public void LearnTypes()
		{
			var a = 5.5;
			var b = 7.8f;
			var c = 0;
			Console.WriteLine(a);
			Console.WriteLine(b);
			Console.WriteLine(c);
			/*uncomment
			... a = 5.5;
			... b = 7.8f;
			... c = 0;
			Console.WriteLine(a);
			Console.WriteLine(b);
			Console.WriteLine(c);
			*/
		}

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("LearnTypes"));
		}
	}
}
