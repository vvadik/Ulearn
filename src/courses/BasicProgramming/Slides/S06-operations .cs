using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using uLearn;

namespace uLearn.Courses.Linq.Slides
{
    public class S06_Operations
    {
        /*

        ##Задача: Исправьте программу так, чтобы она давала корректный ответ. ОБъясните, почему программа не работает.
        */

        [Exercise(SingleStatement = true)]
        [Hint("var is very dangerous...")]
        [ExpectedOutput("5.5")]
        static public void GetSumOfTwoNumbers()
        {
            double a = 5;
            a += 0.5;
            Console.WriteLine(a);
            /*uncomment
            var a = 5;
            a += 0.5;
            Console.WriteLine(a)
            */
        }

        //[Test]
        public void Test()
        {
            var newOut = new StringWriter();
            Console.SetOut(newOut);
            GetSumOfTwoNumbers();
            Assert.AreEqual("5,5", newOut.ToString().Trim());
            }

        /*
        ### Краткая справка
        */
    }
}
