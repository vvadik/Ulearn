using uLearn;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Анализ бинпоиска", "B8FDAE3E-8A17-45A4-8F80-345AE4225EE4")]
	public class S030_BinSearchAnalysis
	{
		//#video pA_UksPvIr8
		/*
		## Заметки по лекции
		*/


		static int FindIndexByBinarySearch(int[] array, int element)
		{
			var left = 0;
			var right = array.Length - 1;
			while (left < right)
			{
				var middle = (right + left) / 2;
				if (element <= array[middle])
					right = middle;
				else left = middle + 1;
			}
			if (array[right] == element)
				return right;
			return -1;
		}
	}
}