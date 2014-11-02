using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Еще примеры", "{6F486E9A-D90F-45B3-8188-CA1F641A9753}")]
	class S030_Example
	{
		//#video vq_v5xd13WU
		/*
		##Заметки по лекции
		*/

		public static int Make(int x, int y)
		{
			if (x <= 0 || y <= 0) return 1;
			return Make(x - 1, y) + Make(x, y - 1);
		}

		/*
		![Карта памяти](_30_map.png)

		Расчет сложности:

		$$p(0,y)=1$$
		$$p(x,0)=1$$
		$$p(x,y)=1+p(x-1,y)+p(x,y-1)$$
		$$C^{x}_{x+y} \le p(x,y)\le 3^{x+y}$$
		$$p(x,y) = \Theta(2^{x+y})$$
		*/
	}
}
