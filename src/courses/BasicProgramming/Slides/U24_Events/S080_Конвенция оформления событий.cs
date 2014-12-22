using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uLearn; 

namespace U24_Events
{
	[Slide("Конвенция оформления событий", "c11c1231-cde0-4c15-808b-0dd83e12fa20")]
	class S080_Конвенция_оформления_событий
	{
		//#video tNK29bPxyhg
		/*
		## Заметки по лекции
		*/

		public class TimerEventArgs : EventArgs
		{
			public int Time { get; set; }
		}

		public delegate void TimerEventHandler(object sender, TimerEventArgs args);


		class Timer
		{
			public int Interval { get; set; }

			public event TimerEventHandler Tick;

			protected virtual void OnTick(object sender, TimerEventArgs args)
			{
				if (Tick != null)
					Tick(sender, args);
			}

			public void Start()
			{
				for (int i = 0; ; i++)
				{
					if (Tick != null) OnTick(this, new TimerEventArgs { Time = i });
					Thread.Sleep(Interval);
				}
			}
		}
    }
}
