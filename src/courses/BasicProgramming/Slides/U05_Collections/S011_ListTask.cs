using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Шифр", "{673C8A47-9560-4458-9BD9-A0C0B58466AA}")]
	public class S011_ListTask
	{
		/*
		Вася этим летом отдыхал на море. А вы? Но просто отдыхать оказалось скучно, и он решил размять мозги. 
		По пути в библиотеку он встретил таинственную незнакомку, которая передала ему письмо.
		В нем лежал лист с текстом и чистый обрывок бумаги. Вася долго пытался понять, что это означает.
		Совсем отчаявшись, он решил сжечь обрывок, но в последний момент заметил, что от жара огня на нем появились буквы.
		Это был алгоритм - как расшифровать таинственное послание:
		вы получили текст, текст состоит из строк. Нужно разбить каждую строку на слова и найти все слова, 
		написанные с большой буквы в том порядке, в каком они появляются в тексте. Объединенные через пробел в строку
 		они откроют вам свою тайну. Для разбиения строки на слова, можно использовать ```Regex.Split(line,@"\W+")```
		Для этого Вася создал функцию ```string DecodeMessage(string[] lines)```, помогите ему завершить ее!
		*/

		[Hint("Вы можете проверить, является ли символ заглавным с помощью ```Char.IsLower(char a)```")]
		[ExpectedOutput("Я Должен Передать Тебе Сообщение О Программировании Точка Даже Если Очень Трудно Тире Не Сдавайся")]
		public static void Main()
		{
			string[] text = LoadText();
			Console.WriteLine(DecodeMessage(text));
		}

		[Exercise]
		private static string DecodeMessage(string[] lines)
		{
			var finalResult = new List<String>();
			foreach (var line in lines)
			{
				var allWords = Regex.Split(line,@"\W+");
				foreach (var word in allWords)
				{
					if (word.Length != 0 && !Char.IsLower(word[0]))
						finalResult.Add(word);
				}
			}
			return string.Join(" ", finalResult.ToArray());
			/*uncomment
			...
			*/
		}

		[HideOnSlide]
		public static string[] LoadText()
		{
			var ans = new string[7];
			ans[0] = "этим дождливым серым и ненастным днем Я пробирался сквозь лес";
			ans[1] = "мне никогда не нравились такие прогулки. но я Должен идти дальше.";
			ans[2] = "мне хотелось бы Передать пару слов о Тебе, друг, но увы";
			ans[3] = "очень интересное Сообщение, подумал я, - О, жестокий мир";
			ans[4] = "Программировании, вероятно, подумал я, Точка";
			ans[5] = "Даже Если Очень Трудно Тире";
			ans[6] = "Не думал я, что скажу ему такое, но я сказал - Сдавайся";
			return ans;
		}
	}
}
