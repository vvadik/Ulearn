using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Строки", "{ad432c0f-e475-4381-9072-0b55ffd574db}")]
	class S040_StringsVideo
	{
		//#video Q62zWcdBCog

		/*
		## Заметки по лекции
		*/
		class Program
		{
			public static void Main()
			{
				//Строки - это последовательности символов
				string myString = "Hello, world!";

				//У строк есть собственные методы и переменные (правильно называть это свойствами),
				//которые позволяют узнать информацию о строке 
				Console.WriteLine(myString.Length);

				myString = myString.Substring(0, 5);
				Console.WriteLine(myString);

				//Тип string может иметь особое значение - null.
				//Это не пустая строка, а отсутствие всякой строки.
				myString = null;

				//Интересно, что тип int такого значения иметь не может.
				//int a=null;
			}
		}
	}
}
