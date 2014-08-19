using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Сравнение for и while", "{65707781-7344-4805-81DA-C4A5199A7412}")]
	class S060_ForVsWhileVideo
	{
		//#video f0KjSjAJ6bA

		/*
		## Заметки по лекции
		*/

		public class Program
		{
			public static void MainX()
			{


				//На самом деле, for(int i=0;i<5;i++) - синтаксический сахар примерно для вот этого:
				int i = 0;
				while (i < 5)
				{
					// инструкции цикла
					i++;
				}

				for (; ; ) //Ктулху-фор - вечный цикл
				{
					break;
				}


				//Да, так тоже можно
				int sum = 0;
				for (
						var line = Console.ReadLine();
						line != "";
						sum += int.Parse(line), line = Console.ReadLine()
						) ;

				/*
				 * for и while теоретически взаимозаменяемы.
				 * Поэтому:
				 *  ИСПОЛЬЗУЙТЕ FOR, если есть четко выделяемая переменная цикла, которая изменяется с каждой итерацией
				 *  ИСПОЛЬЗУЙТЕ WHILE во всех остальных случаях
				 */


			}
		}
	}
}
