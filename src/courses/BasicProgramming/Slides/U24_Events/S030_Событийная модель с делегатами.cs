using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uLearn; 

namespace U24_Events
{
	[Slide("Событийная модель с делегатами", "153a55de-20a9-4f88-9a07-2538a62fd971")]
	class S030_Событийная_модель_с_делегатами
	{
		//#video YnS8KFgnEK4
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
			public static void MainX()
			{
				var timer = new Timer();
				timer.Interval = 500;
				timer.Tick = tickNumber => Console.WriteLine(tickNumber);
				timer.Tick(100);
				timer.Start();
			}
		}
    }
}
