using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Специальные символы", "{9D42AC97-DE5D-4A52-9AB8-B810F105508B}")]
	public class S051_SpecialCharsTask
	{
		/*
		Гуляя по супермаркетам, Вася нашел удивительного робота, который умеет обучаться.
		Вася купил одного из них, и привез домой. Теперь он учить робота говорить слова.
		Но интерфейс динамика робота очень неудобен, он умеет принимать только ```enum```,
		а конвертировать в звук только ```string```.
		Вася написал [нейронную сеть](https://ru.wikipedia.org/wiki/Искусственная_нейронная_сеть), которая умеет
		воспринимать речь людей и возвращать последовательность из ```enum```.
		Вам осталось лишь конвертировать последовательность ```enum``` в одну строку.
		*/

		enum Words
		{
			DoubleQuote, // {"}
			OneQuote, // {'}
			Slash, // {/}
			FavouriteWord, // {I'm\t\a\t\robot} (здесь нет спецсимволов!)
			Tab, // таб
			NewLine // перевод строки
		}

		[ExpectedOutput(@"""
\I'm\t\a\t\robot'	
I'm\t\a\t\robot")]
		public static void Main()
		{
			string robotSaid = ConvertEnumList(GetRobotWords());
			Console.WriteLine(robotSaid);
		}

		[Exercise]
		private static string ConvertEnumList(List<Words> robotWords)
		{
			var str = new StringBuilder();
			foreach (var word in robotWords)
			{
				if (word == Words.DoubleQuote)
					str.Append("\"");
				if (word == Words.OneQuote)
					str.Append("'");
				if (word == Words.FavouriteWord)
					str.Append(@"I'm\t\a\t\robot");
				if (word == Words.Slash)
					str.Append("\\");
				if (word == Words.Tab)
					str.Append("\t");
				if (word == Words.NewLine)
					str.Append("\n");
			}
			return str.ToString();
			/*uncomment
			...
			*/
		}

		[HideOnSlide]
		private static List<Words> GetRobotWords()
		{
			return new List<Words>
			{
				Words.DoubleQuote,
				Words.NewLine,
				Words.Slash,
				Words.FavouriteWord,
				Words.OneQuote,
				Words.Tab,
				Words.NewLine,
				Words.FavouriteWord
			};
		}
	}
}
