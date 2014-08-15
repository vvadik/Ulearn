using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Сравнение и логический тип", "{2C4919E2-42BF-4B32-93DC-5D54A8FDB25C}")]
	class S010_LogicalTypeVideo
	{
		//#video f2nVbV7M_k0

		public static void Main()
		{
			//Сравнение, как и операции сложения или деления, тоже имеет возвращаемое значение
			Console.WriteLine(5 < 4);

			// И его можно сохранить в переменную логического типа
			// Это тип, абсолютно равноправный с int, double и другими 
			bool comparisonResult = 6 > 7;
			Console.WriteLine(comparisonResult);

			//Константы истины и лжи
			bool trueValue = true;
			bool falseValue = false;



			//Все операции сравнения
			Console.WriteLine(5 == 6);
			Console.WriteLine(5 != 6);
			Console.WriteLine(5 < 5);
			Console.WriteLine(5 <= 5);
			Console.WriteLine(5 > 5);
			Console.WriteLine(5 >= 5);
		}

		public static void MainY()
		{
			// Операция "И", или конъюнкция
			Console.WriteLine((5 < 4) && (3 < 4));

			// Операция "ИЛИ", или дизъюнкция
			Console.WriteLine((5 < 4) || (3 < 4));

			// Операция "НЕ", или отрицание
			Console.WriteLine(!(5 < 4));
		}
	}
}
