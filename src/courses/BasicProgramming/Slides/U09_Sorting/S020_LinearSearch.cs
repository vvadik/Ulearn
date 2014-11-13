using uLearn;

namespace uLearn.Courses.BasicProgramming.Slides.U09_Sorting
{
	[Slide("Анализ линейного поиска", "30798679-EE99-417B-BA1A-7577F158820B")]
	public class S020_LinearSearch
	{
		//#video 3bRVv7MFImw
		/*
		## Заметки по лекции
		*/

		static int FindIndex(int[] array, int element)
		{
			for (int i = 0; i < array.Length; i++)
				if (array[i] == element)
					return i;
			return -1;
		}
		static int FindIndex(string[] array, string element)
		{
			for (int i = 0; i < array.Length; i++)
				if (array[i] == element)
					return i;
			return -1;
		}
	}
}