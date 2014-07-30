using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Ошибки в приведении типов", "{0948D588-51EF-4564-9FD2-BC900AA710A2}")]
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
