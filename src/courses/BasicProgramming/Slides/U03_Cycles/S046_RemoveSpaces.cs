using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.Slides.U03_Cycles
{
	[Slide("Убрать пробелы", "{9A15EBD5-0616-4A18-B842-E63E2C3C21FA}")]
	internal class S046_RemoveSpaces
	{
		/*
		Враги вставили в начало каждого полезного текста целую кучу бесполезных пробельных символов!

		Напишите функцию удаления начальных пробелов из строки, чтобы утереть нос врагам.
		*/

		[ExpectedOutput(@"
text
two words
two spaces
tabs
two	tabs
many spaces

line breaks are spaces too
strange spaces
")]
		public static void Main()
		{
			Console.WriteLine(RemoveStartSpaces(" text"));
			Console.WriteLine(RemoveStartSpaces(" two words"));
			Console.WriteLine(RemoveStartSpaces("  two spaces"));
			Console.WriteLine(RemoveStartSpaces("	tabs"));
			Console.WriteLine(RemoveStartSpaces("		two	tabs"));
			Console.WriteLine(RemoveStartSpaces("                             many spaces"));
			Console.WriteLine(RemoveStartSpaces("               "));
			Console.WriteLine(RemoveStartSpaces("\n\r line breaks are spaces too"));
			Console.WriteLine(RemoveStartSpaces(StrangeSpaces + " strange spaces"));
		}

		[HideOnSlide]
		private const string StrangeSpaces = "\x000b\x000c\x00a0\x0085";

		[Exercise]
		[Hint("Вам нужен цикл по символам текста до тех пор пока символы пробельные.")]
		[Hint("На самом деле существует много различных 'пробельных' символов.")]
		[Hint("В классе char есть статический метод для определния, является ли символ пробельным.")]
		[Hint("Вам поможет char.IsWhiteSpace")]
		public static string RemoveStartSpaces(string text)
		{
			var i = 0;
			while (i < text.Length)
			{
				if (!char.IsWhiteSpace(text[i])) return text.Substring(i);
//				if (text[i] != ' ' && text[i] != '\t' && text[i] != '\n' && text[i] != '\r') return text.Substring(i); //wrong
				i++;
			}
			return "";
		}

	}
}
