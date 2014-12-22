using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn;

namespace U19_FunctionalProgramming
{
	[Slide("Делегаты в разборе арифметических выражений", "98a1a41e-ddfd-41e0-ae72-b1e4d8cbb451")]
	class S030_Делегаты_в_разборе_арифметических_выражений
	{
		//#video RoYeCpv19wg
		/*
		## Заметки по лекции
		*/
		static void Main()
		{
			Console.WriteLine(Compute("23+4*"));
		}

		static int Compute(string str)
		{
			var operations = new Dictionary<char, Func<int, int, int>>();
			operations.Add('+', (x, y) => x + y);
			operations.Add('-', (x, y) => x - y);
			operations.Add('*', (x, y) => x * y);
			operations.Add('/', (x, y) => x / y);

			var stack = new Stack<int>();
			foreach (var e in str)
			{
				if (e <= '9' && e >= '0')
					stack.Push(e - '0');
				else if (operations.ContainsKey(e))
					stack.Push(operations[e](stack.Pop(), stack.Pop()));
				else
					throw new ArgumentException();
			}
			return stack.Pop();
		}
	}
}