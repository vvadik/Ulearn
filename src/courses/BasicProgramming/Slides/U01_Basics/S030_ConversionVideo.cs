using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Преобразования типов", "{17fd348e-db6a-4dac-9ef4-c8602931e915}")]
	public class S030_ConversionVideo
	{
		//#video shO5iC4xTmI

		/*
		## Заметки по лекции
		*/

		class Program
		{
			static void Main()
			{
				// Конверсия типов (cast) - это приведение переменных из одного типа в другой 

				int integerNumber = 45;
				double doubleNumber = 34.56;

				doubleNumber = integerNumber;
				// Это неявная конверсия типов: присвоение переменной одного типа 
				// значения переменной другого типа без дополнительных усилий. 
				// Она возможна, когда не происходит потери информации

				integerNumber = (int)doubleNumber;
				// Это явная конверсия типов. В случае, когда конверсия ведет к потере информации
				// (в данном случае - дробной части), необходимо явно обозначать свои намерения
				// по конверсии.
				

				integerNumber = (int)Math.Round(34.67);
				// Округление лучше всего делать не конверсией, а функцией Round. 
				// Кстати, Math - "математическая библиотека" C# - имеет множество других
				// полезных методов. 
				

				long longInteger = 4000000000;
				integerNumber = (int)longInteger;
				// При такой конверсии происходит ошибка переполнения, которая, однако, остается
				// незамеченной для компилятора и среды разработки


				// Таким образом можно отловить эти ошибки явно
				checked
				{
					integerNumber = (int)longInteger;
				}
			}
		}
	}
}
