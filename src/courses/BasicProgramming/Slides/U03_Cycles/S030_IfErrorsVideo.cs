using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Типичные ошибки ветвлений", "{45E4CE73-C9E2-44D9-A5EB-559F3214183B}")]
	class S030_IfErrorsVideo
	{
		//#video _zAA-s8DSdQ

		/*
		## Заметки по лекции
		*/

		public enum Colors
		{
			Red,
			Green,
			Blue
		}

		public class Program
		{
			static string ColorName(Colors color)
			{
				if (color == Colors.Red) return "Красный";
				else if (color == Colors.Blue) return "Синий";
				else return "Зеленый";
			}

			static string ColorNameWrongWay(Colors color)
			{
				if (color == Colors.Red) return "Красный";
				if (color == Colors.Blue) return "Синий";
				if (color == Colors.Green) return "Зеленый";

				return "";
				/* 
				компилятор не знает, что это взаимоисключающие возможности!
				Если закомментировать предыдущую строку, то будет ошибка 
				"not all code paths return a value".
				Т.е. существуют некоторые ветви алгоритма, которые не возвращают значения. 
				Но метод должен вернуть значение в любом случае, поэтому это - ошибка. 
				*/
			}
		}

		public class Program2
		{
			//Часто встречающиеся стилевые ошибки

			//Допустим, вы решили написать метод, возвращающий отрицание переменной
			static bool Negate(bool argument)
			{
				return !argument;
				// так правильно

				/*
				 * if (argument) return false;
				 * else return true;
				 * - а так - нет.
				 */
			}

			//Или метод, сравнивающий два значения
			static bool Compare(int arg1, int arg2)
			{
				return arg1 < arg2;
				//так правильно

				/*
				 * if (arg1<arg2) return true;
				 * else return false;
				 * - а так - нет
				 */
			}

		}
	}
}
