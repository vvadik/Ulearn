using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Переменные", "{fe3a32fb-9f61-4a58-a2f4-547d4c8fac77}")]
	public class S080_LocalAndGlobalVarsVideo
	{
		//#video 1iw5osYSRH0

		/*
		## Заметки по лекции
		*/
		class Program
		{
			static string globalVariable = "Global variable";

			static void MethodA()
			{
				if (globalVariable == "")
				{
					string temporalVariable = "Temporal variable";
					Console.WriteLine(temporalVariable);
				}

				string localVariable = "Local variable";
				
				// Так можно — эта переменная используется в той же области, где и объявлена:
				Console.WriteLine(localVariable);
				
				// Так нельзя — temporalVariable определена только внутри блока if:
				// Console.WriteLine(temporalVariable); 
			}

			static void MethodB()
			{
				// Console.WriteLine(localVariable); //Нельзя — переменная определена в другом методе.
				Console.WriteLine(globalVariable); //Можно — это глобальная переменная

			}
		}
		/* 
		Переменная доступна (грубо) внутри тех фигурных скобок, где она определена.
		Области кода, из которого допустимо обращение к переменной, называется "областью видимости"
		*/
	}
}
