using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uLearn; 

namespace U24_Events
{
	[Slide("Сокращенный синтаксис событий", "9f802d80-d68d-4dd9-b032-13c1a3719f6d")]
	class S070_Сокращенный_синтаксис_событий
	{
		//#video zBzM7Oi_KRw
		/*
		## Заметки по лекции
		*/
		class Timer
		{
			public int Interval { get; set; }

			//Сокращенный синтаксис события.
			//По аналогии с сокращенным синтаксисом свойства, поле создается автоматически
			public event Action<int> Tick;

			public void Start()
			{
				for (int i = 0; ; i++)
				{
					if (Tick != null) Tick(i); 
								//для автособытия можно писать так, но только внутри класса
					Thread.Sleep(Interval);
				}
			}
		}

		public class Program
		{
			public static void Main()
			{
				var timer = new Timer();
				timer.Interval = 500;
				timer.Tick += tickNumber => Console.WriteLine(tickNumber);
				// timer.Tick(100); //так писать по-прежнему нельзя
				timer.Start();

			}
		}
    }
}
