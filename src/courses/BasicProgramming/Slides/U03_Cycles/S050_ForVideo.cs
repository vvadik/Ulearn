using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Циклы for", "{034E49E8-FB81-4973-8FCE-9AE5AEB2046F}")]
	class S050_ForVideo
	{
		//#video ACgYan8WWOM

		/*
		## Заметки по лекции
		*/

		public class Program
		{
			public static void MainX()
			{
				//Суммируем все числа от 1 до 10
				var sum = 0;
				for (int i = 1; i <= 10; i++)
					sum += i;

				// Инкремент
				// Обратите внимание: чтобы сделать что-то 10 раз, мы делаем цикл от 0 до 10!
				// Нумерация в шарпе всегда идет с нуля!
				for (int i = 0; i < 10; i++)
					Console.WriteLine(i);

				// Выводим все символы строки
				string str = "abc";
				for (int i = 0; i < str.Length; i++)
					Console.Write(str[i]);

				//Декремент
				for (int i = 9; i >= 0; i--)
					Console.Write(i + " ");

				Console.WriteLine();
				//Допускается произвольное изменение переменной
				for (int i = 1; i < 100; i *= 2)
					Console.Write(i + " ");

				Console.WriteLine();
				//Не обязательно инициализировать переменную
				int start = int.Parse(Console.ReadLine());
				for (; start >= 0; start--)
					Console.Write(start + " ");

				Console.WriteLine();
				//И условие тоже может быть произвольным
				for (; start * start < 5 * start; start++)
					Console.Write(start + " ");

				Console.WriteLine();

				//Проверка условия осуществляется КАЖДУЮ итерацию цикла! 
				//Поэтому в данном случае лучше ввести переменную var bound=GetBound() и сравнивать с ней,
				//а не вызывать цикл каждый раз
				for (int i = 0; i < GetBound(); i++)
					Console.Write(i + " ");
			}

			static int GetBound()
			{
				Console.Write("!");
				return 5;
			}
		}
	}
}
