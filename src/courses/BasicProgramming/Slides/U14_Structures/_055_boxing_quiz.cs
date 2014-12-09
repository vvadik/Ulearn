using System;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U14_Structures
{
	class _055_boxing_quiz
	{
		//Код для quiz-а

		public struct S
		{
			int A;
		};
		
		public class Program
		{
			static void ShowEquals(object o1, object o2) 
			{
				Console.WriteLine(o1 == o2);
			}

			public static void Main() 
			{
				S s = new S();
				ShowEquals(s, s);
			}
		}

		static void Main()
		{
			object[] s = new object[2];
			s[0] = new S();
			s[1] = s[0];
			Console.WriteLine(s[0] == s[1]);
		}

		[Test]
		public void Test()
		{
			Program.Main();
		}

		[Test]
		public void Test2()
		{
			Main();
		}
	}
}
