using System;
using System.Collections;
using System.Globalization;

namespace uLearn.Courses.BasicProgramming.Slides.U11_Inheritance
{
	[Slide("По часовой стрелке", "{E147BFF3-7BC6-454B-A6E4-BFE9E9DE6D41}")]
	public class S016_ClockwiseComparer : SlideTestBase
	{
		/*
		А как насчет того, чтобы отсортировать точки в порядке следования по часовой стрелке,
		считая первой ту, что находится в полудне?
		*/

		[ExpectedOutput(
@"0 1
0.01 1
1 0
0 -1
-1 0")]
		static void Main()
		{
			var array = new[]
			{
				new Point { X=1, Y=0 },
				new Point { X=-1, Y=0},
				new Point { X=0, Y=1},
				new Point { X=0, Y=-1},
				new Point { X=0.01, Y=1 }
			};
			Array.Sort(array, new ClockwiseComparer());
			foreach (var e in array)
				Console.WriteLine("{0} {1}", e.X, e.Y);
		}

		class Point
		{
			public double X;
			public double Y;
		}

		[ExcludeFromSolution]
		[HideOnSlide]
		public class ClockwiseComparer : IComparer
		{
			double GetOrdinal(Point p)
			{
				return -(Math.Atan2(p.Y,p.X)+Math.PI+Math.PI/4)%(2*Math.PI);
			}
			public int Compare(object x, object y)
			{
				return GetOrdinal(x as Point).CompareTo(GetOrdinal(y as Point));
			}
		}

	}
}