using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	class SomeClass {
		public static int s = 1;
		public int d = 1;

		public void Run() {
			Console.Write(s + " " + d + " ");
			s++; d++;
		}

		public static void Main() {
			var object1 = new SomeClass();
			var object2 = new SomeClass();
			object1.Run();
			object2.Run();
			object1.Run();
		}
	}
}
