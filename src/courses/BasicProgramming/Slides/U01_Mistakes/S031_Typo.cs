using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U01_Mistakes
{
	[Slide("Очепятки", "{7E86A275-9B84-4071-A0A3-2D1FE17B0DF1}")]
	class S031_Typo : SlideTestBase
	{
		/*
		Вася нашел крутое приложение на телефон. 
		Можно фотографировать текст и преобразовывать фотографию в текстовый документ.
		
		Вася тут же сфотографировал код с экрана своего одногруппника и с сожалением обнаружил, что результат не компилируется.
		По-видимому, некоторые символы распознались неверно.
		
		Исправьте все ошибки так, чтобы код работал правильно.
		*/

		[Exercise]
		[ExpectedOutput("Hello, World!\r\n12.5")]
		[Hint("Вспомните, как работает `var`")]
		public static void Main()
		{
			Console.WriteLine("Hello, World!");
			var number = 5.5;
			number += 7;
			Console.WriteLine(number);
			/*uncomment
			Console.WriteLine{'Hello, World!'};
			var numher = 5,5;
			number += 7;
			Console.Writeline(number):
			*/
		}
	}
}
