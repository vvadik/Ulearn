using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using uLearn;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[TestFixture]
	public class S07_Methods
	{
		/*

		##Задача: Методы
		Как многие из вас знают - ответ на самый главный вопрос человечества - 42. Но, что будет, если это знание возвести в квадрат?
		Узнайте его, реализуя методы Print, GetSquare. А затем узнайте, что было в этот год и поймите истинную природу божественности 42.
		*/

		[Exercise(SingleStatement = true)]
		[ExpectedOutput("1764")]
		static public void MainX()
		{
			Print(GetSquare(42));
			/*uncomment
			Print(GetSquare(42));
			*/
		}

		static private int GetSquare(int i)
		{
			return i * i;
		}

		static private void Print(int number)
		{
			Console.WriteLine(number);
		}

		[Test]
		public void Test()
		{
			TestExerciseStaff.TestExercise(GetType().GetMethod("MainX"));
		}
	}
}
