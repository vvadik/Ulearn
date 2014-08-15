using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U02_Mistakes
{
	[Slide("Отладка", "{B591027E-9E13-4125-8A4F-7A432FBCEA7D}")]
	class S050_DebugVideo
	{
		//#video AzD_1S9UR2A

		/*
		Вот пример кода, который сгенерирует Null references exception
		*/

		class Program
		{
			static string myString;

			static void MainX()
			{
				Console.WriteLine(myString[0]);
			}
		}

		/*
		А это пример кода, генерирующего InnerException.
		В ситуации вложенного исключения нужно обращать внимание на InnerException.
		Конкретно для данной ситуации нужно избегать сложных действий в инициализации.
		*/

		class Program2
		{
			static int variable = int.Parse(Console.ReadLine());

			static void Main()
			{
				Console.WriteLine(variable);
			}
		}
	}
}
