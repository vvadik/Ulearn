using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Числовые типы данных", "{f1953527-ac7a-4bb7-93fc-14bb9d2de249}")]
	public class S020_TypesVideo
	{
		//#video shO5iC4xTmI

		/*
		## Заметки по лекции
		*/

		class Program
		{
			static void Main()
			{
				//Переменная — это именованная область памяти.
				//
				//Тип переменной — это формат области памяти, определяющий множество возможных значений
				// переменной и множество допустимых операций над ней.

				int integerNumber;
				// так объявляется переменная: тип (int), затем имя (integerNumber)

				// так осуществляется присваивание           
				integerNumber = 10;

				// double - основной тип чисел с плавающей точкой.
				// Можно совмещать объявление и присваивание.
				double realNumber = 12.34;

				// float - тип меньшей точности.
				// Суффикс f говорит, что 1.234 - константа типа float, а не double.
				// Используются в библиотеках работы с графикой в Windows.
				float floatNumber = 1.234f;

				//long (большие целые числа). Используются в основном для подсчета милисекунд.
				long longIntegerNumber = 3000000000000;

				// Есть и другие типы данных: short, decimal, и т.д.
				// В основном, для чисел вы будете пользоваться int и double, иногда - long и float
			}
		}
		/*
		Вы можете познакомиться с полным списком встроенных типов данных в главе учебника по C#
		["Встроенные типы данных"](http://msdn.microsoft.com/ru-ru/library/cs7y5x0x(v=vs.90).aspx)
		*/
	}
}