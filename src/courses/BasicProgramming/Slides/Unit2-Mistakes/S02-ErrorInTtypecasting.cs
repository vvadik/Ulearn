using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Ошибки в приведении типов")]
	class S02_ErrorInTtypecasting
	{
		public static void Main()
		{
			double number = Magic((int)4.3, (int)2.2);
		}

		private static double Magic(int i, int r)
		{
			throw new Exception();
		}
	}
}
