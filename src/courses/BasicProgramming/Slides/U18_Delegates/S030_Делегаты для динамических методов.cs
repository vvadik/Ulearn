using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U18_Delegates
{
	[Slide("Делегаты для динамических методов", "6582cd95-69de-462e-a4e8-06ef6f9865d8")]
	class S030_Делегаты_для_динамических_методов
	{
		//#video 6yQEfvaJIxc
		/*
		## Заметки по лекции
		*/

		public delegate int StringComparer(string x, string y);

		public class Program
		{

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

				//Можно без проблем указывать на динамические методы.
				//Для того, чтобы указать на метод, нужно обратиться к нему как для вызова
				//(с указанием объекта для динамических методов, указанием класса 
				// для статических методов, определенных в другом классе),
				//НО БЕЗ КРУГЛЫХ СКОБОК.
				var simpleComparer =
					new StringComparer(obj.CompareStrings);
				SortStrings(strings, simpleComparer);
			}

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

			
		}

    }
}
