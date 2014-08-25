using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Рекламы", "{741D39BD-D543-40D2-ABBC-941C7F778106}")]
	public class S041_StringBuilderTask
	{
		/*
		Во время отпуска Вася часто навещался в местный супермаркет. Каждый день там крутили одни и те-же рекламы.
		Вася записал текст каждой рекламы.
		Теперь ему интересно, сколько символов бесполезной информации он услышал за все время посещения супермаркета,
		если каждую рекламу он услышал ровно ```Count``` раз.
		Функцию ```GetAllAdvertisement``` реализовывать не нужно, её уже сделал Вася.
		*/

		public const int Count = 21000;

		[ExpectedOutput("2457000")]
		public static void Main()
		{
			var charsCount = 0;
			foreach (StringBuilder advertisement in GetAllAdvertisement())
			{
				StringBuilder longAdvertisement = ConcatAdvertisement(advertisement, Count);
				charsCount += longAdvertisement.Length;
			}
			Console.WriteLine(charsCount);
		}

		[Exercise]
		private static StringBuilder ConcatAdvertisement(StringBuilder advertisement, int count)
		{
			var str = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				str.Append(advertisement);
			}
			return str;
			/*uncomment
			return advertisement;
			*/
		}

		[HideOnSlide]
		[Hint("Разумно будет получить одну строку - представление всей услышанной информации")]
		[Hint("Длина этой строки и будет ответом")]
		private static IEnumerable<StringBuilder> GetAllAdvertisement()
		{
			yield return new StringBuilder("New IPhone 94! Now!");
			yield return new StringBuilder("New IPhone 95s! Now!");
			yield return new StringBuilder("New IPhone 96! Now!");
			yield return new StringBuilder("New IPhone 97! Now!");
			yield return new StringBuilder("New IPhone 98s! Now!"); 
			yield return new StringBuilder("New IPhone 99s! Now!");
		}
	}
}
