using System;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U11_Inheritance
{
    [Slide("Поиск минимума", "{9E6C6FE1-9282-4ABC-853C-5CE6FB5BFA76}")]
    public class S055_Min : SlideTestBase
    {
        /*
        Помните задачу "Среднее трех"? Пришло время повторить ее для общего случая!       
        */
        [ExpectedOutput(
@"2
A
2")]
        [Hint("Аргументами метода должны быть IComparable")]
        [Hint("И возвращаемым значением тоже")]
        public static void Main()
        {
            Console.WriteLine(Min(new[] {3, 6, 2, 4}));
            Console.WriteLine(Min(new[] {"B", "A", "C", "D"}));
            Console.WriteLine(Min(new[] {'4', '2', '7'}));
        }

        
        [ExcludeFromSolution]
        [HideOnSlide]
        static object Min(Array args)
        {
            IComparable min = args.GetValue(0) as IComparable;
            for (int i = 1; i < args.Length; i++)
            {
                if (min.CompareTo(args.GetValue(i)) >= 0)
                    min = args.GetValue(i) as IComparable;
            }
            return min;
        }
    }
}