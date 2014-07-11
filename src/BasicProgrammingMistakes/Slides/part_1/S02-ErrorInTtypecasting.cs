using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicProgrammingMistakes.Slides.part_1
{
	class S02_ErrorInTtypecasting
	{
		public static void Main()
		{
			double number = Magic((int)4.3, (int)2.2);
		}

		private static double Magic(int i, int r)
		{
			double a;
			throw new Exception();
		}
	}
}
