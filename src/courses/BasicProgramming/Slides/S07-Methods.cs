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

namespace uLearn.Courses.Linq.Slides
{
    public class S07_Methods
    {
        /*

        ##Задача: Реализуйте методы.
        */

        [Exercise(SingleStatement = true)]
        [Hint("var is very dangerous...")]
        [ExpectedOutput("49")]
        public void MainX()
        {
            Print(GetSquare(7));
            /*uncomment
                Print(GetSquare(7));
            */
        }

        private int GetSquare(int i)
        {
            return i*i;
        }

        private void Print(int number)
        {
            Console.WriteLine(GetSquare(number));
        }

        /*
        ### Краткая справка
        */
    }
}
