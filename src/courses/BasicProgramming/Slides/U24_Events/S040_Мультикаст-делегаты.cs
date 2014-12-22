using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uLearn; 

namespace U24_Events
{
	[Slide("Мультикаст-делегаты", "5cd76def-cd88-4966-8251-d8bdda78a448")]
	class S040_Мультикаст_делегаты
	{
		//#video AwkIgGBPa1c
		/*
		## Заметки по лекции
		*/

		class Timer
		{
			public int Interval { get; set; }
			public Action<int> Tick;
			public void Start()
			{
				for (int i = 0; ; i++)
				{
					Tick(i);
					Thread.Sleep(Interval);
				}
			}
		}

		public class Program
		{
			public static void Log(int value)
			{
				Console.WriteLine("Logged value " + value);
			}

			public static void Main()
			{
				var timer = new Timer();
				timer.Interval = 500;

				//Делегат может указывать на несколько методов сразу.
				//Таким образом можно указывать несколько обработчиков для одного события.
				timer.Tick += tickNumber => Console.WriteLine(tickNumber);
				timer.Tick += Log;

				timer.Start();

				//Как работают функции?
				Func<int, int> f = null;
				f += z => z * 2;
				f += z => z * 3;
				Console.WriteLine(f(1)); //напечатает 3, возвращается последнее значение

			}
		}
    }
}
