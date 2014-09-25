using System;
using System.Collections.Generic;
using System.Text;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	[Slide("Снова незнакомка", "{741D39BD-D543-40D2-ABBC-941C7F778106}")]
	public class S041_StringBuilderTask : SlideTestBase
	{
		/*
		Незнакомка вернулась!

		На рабочем столе своего ноутбука Вася обнаружил огромный файл, начинающийся так:

			push Привет! Это снова я! Пока!
			pop 5
			push Как твои успехи? Плохо?
			push qwertyuiop
			push 1234567890
			pop 26
			...

		Да, кажется предыдущая программа по расшифровке шифра не понадобится — незнакомка не повторяется...

		Вася где-то слышал, что pop и push — это операции работы со стеком.
		Видимо, тут нужно действовать по аналогии — push дописывает указанную строку в конец текста, а pop удаляет из конца указанное количество символов.
		
		Попробовав выполнить первые шесть операций, Вася получил текст:

			Привет! Это снова я! Как твои успехи?

		Видимо, чтобы прочитать второе послание незнакомки, нужно выполнить все операции из файла. 
		Но файл слишком большой, тут без программы-декодировщика не обойтись!
		*/

		[ExpectedOutput(@"Результат декодирования:
Привет! Это снова я! Надеюсь ты поправился после солнечного удара. Если ты читаешь это, значит ты научился эффективно работать со строками! Молодец, продолжай в том же духе!")]
		[HideOnSlide]
		[HideExpectedOutputOnError]
		public static void Main()
		{
			var message =
				"Привет! Это снова я! Надеюсь ты поправился после солнечного удара. Если ты читаешь это, значит ты научился эффективно работать со строками! Молодец, продолжай в том же духе!";
			var commands = Encode(message);
			var decoded = ApplyCommands(commands);
			Console.WriteLine("Результат декодирования:");
			if (decoded.Length > 200)
				decoded = decoded.Substring(0, 200) + "...";
			Console.WriteLine(decoded);
			if (decoded != message)
				throw new Exception("Ошибка в декодировании: " + decoded);
		}
		[HideOnSlide]
		private static string[] Encode(string message)
		{
			var parts = message.Split(' ');
			var commands = new List<string>();
			foreach (var part in parts)
			{
				commands.Add("push " + part + " да!");
				commands.Add("pop 3");
				AddRandomPart(commands, 200000);
			}
			commands.Add("pop 1");
			return commands.ToArray();
		}

		[HideOnSlide]
		private static void AddRandomPart(List<string> commands, int len)
		{
			for(int i=0; i<len/4; i++)
				commands.Add("push haha");
			commands.Add("pop " + len);
		}

		[Exercise]
		[Hint(
			"Возможно вам понадобятся методы `command.IndexOf(' ')` для поиска индекса первого пробела и `command.Substring` для взятия подстроки"
			)]
		[Hint("Массовы операции со строками эффективнее делать с помощью специального класса `StringBuilder`")]
		private static string ApplyCommands(string[] commands)
		{

			var str = new StringBuilder();
			foreach (var command in commands)
			{
				var spaceIndex = command.IndexOf(' ');
				var cmd = command.Substring(0, spaceIndex);
				var arg = command.Substring(spaceIndex+1);
				if (cmd == "push")
					str.Append(arg);
				else if (cmd == "pop")
				{
					var len = int.Parse(arg);
					str.Remove(str.Length - len, len);
				}
			}
			return str.ToString();
		}
	}
}
