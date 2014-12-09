using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn;

namespace U18_Delegates
{
	[Slide("Замыкание", "30ddfb02-ddef-4122-b305-28e403f935ab")]
	class S100_Замыкание
	{
		//#video wR4WyEVWYwA
		/*
		## Заметки по лекции
		*/

		public static void Sort<T>(T[] array, Func<T, T, int> comparer)
		{
			for (int i = array.Length - 1; i > 0; i--)
				for (int j = 1; j <= i; j++)
				{
					var element1 = array[j - 1];
					var element2 = array[j];
					if (comparer(element1, element2) > 0)
					{
						var temporary = array[j];
						array[j] = array[j - 1];
						array[j - 1] = temporary;
					}
				}
		}

		static void MainX()
		{
			//Когда локальная переменная используется внутри лямбда-выражения - это замыкание
			bool Descending = true;
			Func<string, string, int> cmp =
				(x, y) => x.CompareTo(y) * (Descending ? -1 : 1);
			var strings = new[] { "A", "B", "AA" };
			Sort(strings, cmp);
			Descending = false;
			Sort(strings, cmp);
		}
	}
}
