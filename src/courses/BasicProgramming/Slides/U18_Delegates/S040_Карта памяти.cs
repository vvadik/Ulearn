using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U18_Delegates
{
	[Slide("Карта памяти", "17bf0a3c-4089-4c3f-8e06-0c64ac72ea42")]
	class S040_Карта_памяти
	{
		//#video cWFFCfoFD6I
		/*
		## Заметки по лекции

		![Карта памяти](map1.png)
		*/

		public delegate int StringComparer(string x, string y);

		public class Program
		{
			public static void SortStrings(string[] array, StringComparer comparer)
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


			static int CompareStringLength(string x, string y)
			{
				return x.Length.CompareTo(y.Length);
			}

			class Comparer
			{
				public bool Descending { get; set; }
				public int CompareStrings(string x, string y)
				{
					return x.CompareTo(y) * (Descending ? -1 : 1);
				}
			}

			static void Main()
			{
				var strings = new[] { "A", "B", "AA" };
				var lengthComparer =
					new StringComparer(CompareStringLength);
				SortStrings(strings, lengthComparer);
				var obj = new Comparer { Descending = true };
				var simpleComparer =
					new StringComparer(obj.CompareStrings);
				SortStrings(strings, simpleComparer);
			}
		}
    }
}
