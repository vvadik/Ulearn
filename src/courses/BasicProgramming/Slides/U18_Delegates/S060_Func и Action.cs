using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U18_Delegates
{
	[Slide("Func и Action", "3e04fc25-d39c-441e-bcf2-c3789d56c4c3")]
	class S060_Func_и_Action
	{
		//#video bJhTyn3adgk
		/*
		## Заметки по лекции
		*/


		//Можно использовать уже предопределенные делегатные типы: Func и Action.
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
			Sort(strings, CompareStringLength);
			var obj = new Comparer { Descending = true };
			Sort(strings, obj.CompareStrings);
		}
		
    }
}
