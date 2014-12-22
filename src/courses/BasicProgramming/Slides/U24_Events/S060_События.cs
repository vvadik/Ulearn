using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uLearn; 

namespace U24_Events
{
	[Slide("События", "d679af5e-8646-4bf0-9002-0cb74ac59262")]
	class S060_События
	{
		//#video CHtf1rJ6sXo
		/*
		## Заметки по лекции
		*/

		class Timer
		{
			private int interval;
			public int Interval
			{
				get { return interval; }
				set { interval = value; }
			}

			private Action<int> tick;

			//Событие - это пара методов add и remove
			//для подключения обработчика, по аналогии со свойствами
			public event Action<int> Tick
			{
				add { tick += value; }
				remove { tick -= value; }
			}

			public void Start()
			{
				for (int i = 0; ; i++)
				{
					if (tick != null) tick(i);
					//Tick(i) написать нельзя. Событие нельзя вызвать!
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
				timer.Tick += tickNumber => Console.WriteLine(tickNumber);
				// timer.Tick = tickNumber => Console.WriteLine(tickNumber); 
					//нельзя, т.к. обработчик можно только добавить или удалить
				// timer.Tick(100); 
					//нельзя, событие нельзя вызвать 
				// timer.tick(100);
					//нельзя, т.е. поле приватное
				timer.Start();

			}
		}
    }
}
