using System;
using System.Collections.Generic;
using uLearn;

namespace Slide02
{
	[Slide("Словари", "{CA89802F-7462-4706-9AAF-51E76012D255}")]
	public class S020_Dictionary
    {
		//#video 1WaWDgBxyYc
		/*
		## Заметки по лекции
		*/
        static void Main()
        {
            // Массивы и листы позволяют нам установить соответствие между числом 
            // (индексом массива) и чем-то: например, массив string[] по числу дает доступ
            // к строке.
            // Иногда хочется установить, например, соответствие между строкой и числом.

            // Задача: дан массив строк, подсчитать для каждой строки количество вхождений

            var array = new[] { "A", "AB", "B", "A", "B", "B" };

            //Удобно делать это с помощью словаря - сущности, которая ассоциирует между собой
            //значения любых типов
            var dictionary = new Dictionary<string, int>();

            foreach (var e in array)
            {
                if (!dictionary.ContainsKey(e)) dictionary[e] = 0;
                dictionary[e]++;
            }

            foreach (var e in dictionary)
            {
                Console.WriteLine(e.Key + "\t" + e.Value);
            }

        }
    }
}