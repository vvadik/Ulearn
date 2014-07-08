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
    public class S07_Methods
    {
        /*

        ##Задача: Реализуйте методы.
        */

        [Exercise(SingleStatement = true)]
        [ExpectedOutput("49")]
        static public void MainX()
        {
            Print(GetSquare(7));
            /*uncomment
                Print(GetSquare(7));
            */
        }

        static private int GetSquare(int i)
        {
            return i*i;
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
        /*
        ### Краткая справка
        */
    }
}
