using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U18_Delegates
{
	[Slide("Ловушка замыкания", "a16012ed-340d-4666-a0b2-7a2288d84391")]
	class S120_Ловушка_замыкания
	{
		//#video HG7b58AIS-Y
		/*
		## Заметки по лекции
		*/
		static void Main()
		{
			var functions = new List<Func<int, int>>();

			for (int i = 0; i < 10; i++)
				functions.Add(x => x + i);

			//Этот код выведет десять раз "10", потому что i ушла в замыкание
			//и общая для всех лямбд в списке
			foreach (var e in functions)
				Console.WriteLine(e(0));

			functions = new List<Func<int, int>>();

			for (int i = 0; i < 10; i++)
			{
				var j = i;
				functions.Add(x => x + j);
			}

			//Этот код выведет числа от 0 до 10,
			//потому что j - локальная для цикла,
			//и соответственно своя на каждой итерации и у каждой лямбды
			foreach (var e in functions)
				Console.WriteLine(e(0));
		}
    }
}
