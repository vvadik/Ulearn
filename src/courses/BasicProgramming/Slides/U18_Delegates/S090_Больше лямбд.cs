using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn;

namespace U18_Delegates
{
	[Slide("Больше лямбд", "cb3de1b1-e96d-4184-be48-069ad78c3164")]
	class S090_Больше_лямбд
	{
		//#video vBIzxMSXPkE
		/*
		## Заметки по лекции
		*/
		static Random rnd = new Random();

		static void Main()
		{
			Func<int, int> f = x => x + 1;

			Console.WriteLine(f(1));

			Func<int> generator = () => rnd.Next();

			Console.WriteLine(generator());

			Func<double, double, double> h = (a, b) =>
				{
					b = a % b;
					return b % a;
				};

			Action<int> print = x => Console.WriteLine(x);

			print(generator());

			Action printRandomNumber = () => Console.WriteLine(rnd.Next());
			Action printRandomNumber1 = () => print(generator());
		}
	}
}