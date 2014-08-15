using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Ktulhu for", "{C65BE127-D5E6-46B6-9B5C-A6B1229461A4}")]
	class S10_KtulhuFor
	{
		/*
		Ктулху наступает, и только вы можете его остановить! Сможете ли вы сотворить заклинание,
		используя магию программирования? Функция ```CastSpellForKtulhu``` принимает первым аргументом 
		свиток для заклинания, вторым заклинание, а третьим - сколько раз должно быть написано заклинание на свитке. 
		Возвращает уже заполненный свиток.
		Но вот незадача - силы Ктулху столь сильны, что он ограничил ваши возможности, и вы не можете создавать дополнительные
		переменные. Удачи! 
		*/

		[ExpectedOutput("Fus\r\nRoRo\r\nDahDahDah")]
		public static void Main()
		{
			Console.WriteLine(CastSpellForKtulhu("", "Fus", 1));
			Console.WriteLine(CastSpellForKtulhu("", "Ro", 2));
			Console.WriteLine(CastSpellForKtulhu("", "Dah", 3));
		}

		[Exercise]
		public static string CastSpellForKtulhu(string paper, string spell, int count)
		{
			for (; count > 0; count--)
				paper += spell;
			return paper;
			/*uncomment
			 return paper;
			*/
		}
	}

	
}
