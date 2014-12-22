using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U19_FunctionalProgramming
{
	[Slide("Лямбда-выражения в тестах", "ce3f4151-f189-4f7c-898f-07d44a04f907")]
	class S050_Лямбда_выражения_в_тестах
	{
		//#video JLghL3Tj6Z8
		/*
		## Заметки по лекции
		*/
		public class Robot
		{
			public double X { get; set; }
			public double Y { get; set; }

			public double Angle { get; set; }
			public void Move(double dx)
			{
				X += dx * Math.Cos(Angle);
				Y += dx * Math.Sin(Angle);
			}
			public void Rotate(double da)
			{
				Angle += da;
			}
		}

		public class Program
		{
			public static Action<Robot> GenerateTest(double X, double Y, double angle, params double[] commands)
			{
				return robot =>
				{
					for (int i = 0; i < commands.Length; i++)
						if (i % 2 == 0)
							robot.Move(commands[i]);
						else robot.Rotate(commands[i]);
					Debug.Assert(X == robot.X);
					Debug.Assert(Y == robot.Y);
					Debug.Assert(angle == robot.Angle);
				};
			}
			public static void Main()
			{
				var tests = new List<Action<Robot>>();
				tests.Add(robot =>
				{
					robot.Move(10);
					Debug.Assert(10 == robot.X);
				});
				tests.Add(robot =>
				{
					robot.Move(-10);
					Debug.Assert(-10 == robot.X);
				});
				tests.Add(GenerateTest(10, 10, Math.PI / 2, 10, Math.PI / 2, 10));

				foreach (var e in tests)
				{
					var robot = new Robot();
					e(robot);
				}
			}
		}
    }
}
