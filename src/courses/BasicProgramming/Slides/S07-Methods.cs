using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using uLearn;

namespace uLearn.Courses.Linq.Slides
{
    public class S07_Methods
    {
        /*

        ##Задача: Исправьте программу так, чтобы она давала корректный ответ. ОБъясните, почему программа не работает.
        */

        [Exercise(SingleStatement = true)]
        [Hint("var is very dangerous...")]
        public double GetSumOfTwoNumbers()
        {
            double a = 5;
            a += 0.5;
            return a;
            /*uncomment
            var a = 5;
            a += 0.5;
            Console.WriteLine(a)
            */
        }

        [Test]
        public void Test()
        {
            var number = GetSumOfTwoNumbers();
            Assert.That(
                5.5,
                Is.EqualTo(number));
        }

        /*
        ### Краткая справка
        */
    }
}
