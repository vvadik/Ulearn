using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U18_Delegates
{
	[Slide("Лямбда-выражения", "5cd3c541-285d-4b50-bc8e-64815c188be4")]
	class S080_Лямбда_выражения
	{
		//#video IO69OVNqswU
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

		static void Main()
		{
			var strings = new[] { "A", "B", "AA" };
			//Лямбда-выражение - еще более краткая форма записи.
			//Теперь компилятор догадывается не только до типа возвращаемого значения,
			//но и до типа аргументов. 
			//Обратите внимание, что типы во всей программе выводятся из строки с 
			//объявлением массива!
			Sort(strings, (x, y) => x.Length.CompareTo(y.Length));
		}
    }
}
