using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slide02
{
	class Program
	{
		public static void MainX()
		{
			int a = 10;
			//a += 0.5; //раскомментируйте, чтобы получить ошибку
		}
	}
}

// Ошибка: Cannot implicitly convert type 'double' to 'int'. An explicit conversion exists (are you missing a cast?)	
// Здесь среда даже подсказывает вам: не забыли ли вы cast?