using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Подмножества", "{CBCDCD2C-CF86-4835-A4F6-B170CEE07CEF}")]
	class S050_Subsets
	{
		//#video QS-opNme290
		/*
		## Заметки по лекции
		*/
		static int[] weights = new int[] { 2, 5, 6, 2, 4, 7 };

        static void Evaluate(bool[] subset)
        {
            var delta = 0;
            for (int i = 0; i < subset.Length; i++)
                if (subset[i]) delta += weights[i];
                else delta -= weights[i];
            foreach (var e in subset)
                Console.Write(e ? 1 : 0);
            Console.Write(" ");
            if (delta == 0)
                Console.Write("OK");
            Console.WriteLine();
        }

        static void MakeSubsets(bool[] subset, int position)
        {
            if (position == subset.Length)
            {
                Evaluate(subset);
                return;
            }
            subset[position] = false;
            MakeSubsets(subset, position + 1);
            subset[position] = true;
            MakeSubsets(subset, position + 1);
        }
        static void MainX()
        {
            MakeSubsets(new bool[weights.Length], 0);
        }
	}
}
