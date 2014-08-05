using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Очепятки", "{7E86A275-9B84-4071-A0A3-2D1FE17B0DF1}")]
	class S03_Typo
	{
		/*
		##Задача: Очепятки
		
		Вы установили крутое приложение на телефон. Можно фотографировать код на экране, 
		и преобразовывать фотографию в текстовый документ с кодом!
		Жаль только, что приложение пока что не на столько точно распознает символы, чтобы код сразу компилировался.
		Сделйте так, чтобы код компилировался и писал нужные строки.
		*/

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
