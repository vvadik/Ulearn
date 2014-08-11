using System; //закомментируйте эту строчку, чтобы получить ошибку

namespace Slide01
{
	class Program
	{
		public static void MainX()
		{
			Console.Write("Hello, world!");
		}
	}
}
// Ошибка: The name 'Console' does not exist in the current context
// Решение: правой кнопкой на Console -> Resolve -> using System;