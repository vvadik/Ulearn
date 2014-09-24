using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	Вася начал писать функцию перевода цифр в текст, но ему надоело...

	Кажется, эту задачу можно решить немного веселее, с помощью массивов.
	Возможно даже всего в одну строчку кода.
	
	Сделайте это!
	*/
	[Slide("Цифры прописью", "{0A58245F-518C-4979-84B2-E10E05A50593}")]
	class S021_DigitsToText
	{
		[ExpectedOutput(@"
ноль
один
два
три
четыре
пять
шесть
семь
восемь
девять
")]
		public static void Main()
		{
			// тестовый код:
			for(int digit=0; digit <10; digit++)
				Console.WriteLine(DigitToText(digit));
		}

		[Exercise]
		[SingleStatementMethod]
		[Hint("Подумайте, как тут можно применить сокращенный синтаксис создания массивов вместо if-ов.")]
		[Hint("Между переданной цифрой и индексом массива есть связь")]
		private static string DigitToText(int digit)
		{
			return new[] { "ноль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" }[digit];
			/*uncomment
			if (digit == 0) return "ноль";
			else if (digit == 1) return "один";
			else if (digit == 2) return "два";
			return "надоело...";
			*/
		}
	}
}
