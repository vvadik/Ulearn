using System;
using System.Drawing;
using System.Windows.Forms;
using uLearn;

namespace OOP.Slides.U02_Polymorphism
{
	[Slide("Задача: Визуализация *", "8FBDA388-C8E7-43B3-9C1E-FE387FEA6139")]
	public class S060_Visualize
	{
		/*
		Эта задача необязательная. Она предполагает самостоятельное изучение средств вывода графики в платформе .NET.

		Если подключить к проекту сборки `System.Windows.Forms` и `System.Drawing` (Project → References → Add reference → Framework), 
		то станут доступны классы для работы с оконным интерфейсом и графикой.

		Вот так можно определить класс, представляющий окно с возможностью рисовать на нем:
		
		*/
		public class DrawingForm : Form
		{
			private int x = 0; // это поле лишь для примера анимации. У вас должно быть что-то свое.
			private Timer timer;

			public DrawingForm()
			{
				WindowState = FormWindowState.Maximized; // распахнем окно на весь экран

				timer = new Timer { Interval = 30, Enabled = true };
				timer.Tick += OnTick; // Функция OnTick теперь будет вызываться каждые 30 миллисекунд

				DoubleBuffered = true; // Избавимся от мерцания при перерисовке содержимого окна
			}

			private void OnTick(object sender, EventArgs e)
			{
				x += 2;
				if (x > ClientRectangle.Width*2)
					timer.Stop(); // так можно остановить таймер, чтобы OnTick больше не вызывался.
				Invalidate(); // так можно заставить окно перерисовать свое содержимое.
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				// Этот метод вызывается каждый раз, когда окно решает, что нужно перерисовать свое содержимое. 

				base.OnPaint(e);
				e.Graphics.FillRectangle(Brushes.Black, ClientRectangle);
				e.Graphics.FillEllipse(Brushes.Yellow, x % ClientRectangle.Width, 100, 50, 20);
			}
		}

		/*
		А так можно создать и открыть это окно:
		*/

		public void OpenWindow()
		{
			var form = new DrawingForm();
			form.ShowDialog(); // тут программа приостановит выполнение до тех пор, пока вы не закроете окно.
		}

		/*
		### Задача
		
		Создайте отдельный проект визуализатора движения вашего робота к цели. 
		
		Постарайтесь избежать дублирования кода в визуализаторе и консольном проекте, измеряющем эффективность. Проведите необходимый для этого рефакторинг кода.
		*/
	}
}