using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uLearn; 

namespace U24_Events
{
	[Slide("Целостность событийной модели", "86eea064-3fd9-4437-a7df-d9d5e72808e2")]
	class S050_Целостность_событийной_модели
	{
		//#video zYq7zSIGDyk
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
				//Проблема: никто не помешает написать так:
				timer.Tick(100);
				//Это нарушает целостность событийной модели.
				//Введение свойств не спасает: если есть доступ на чтение, есть и доступ на вызов
				//А именно его нужно закрыть.
				timer.Start();
			}
		}
    }
}
