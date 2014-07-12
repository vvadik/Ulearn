using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Очепятки")]
	class S03_Typo
	{
		/*
		##Задача: Очепятки
		
		Вы установили крутое приложение на телефон. Можно фотографировать код на экране, 
		и преобразовывать фотографию в текстовый документ с кодом!
		Жаль только, что приложение пока что не на столько точно распознает символы, чтобы код сразу компилировался.
		Сделйте так, чтобы код компилировался и писал нужные строки.
		*/

		public static int Satiety = 0;


		[Exercise]
		[ExpectedOutput("Hello, World!\r\n12.5")]
		public static void Main()
		{
			Console.WriteLine("Hello, World!");
			var number = 5.5;
			number += 7;
			Console.WriteLine(number);
			/*uncomment
			Console.WriteLine{'Hello, World!'};
			var number = 5,5;
			number += 7;
			Console.Writeline(number):
			*/
		}
	}
}
