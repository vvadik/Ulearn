using System;
using System.Security;

namespace CourseToolHotReloader.Log
{
	public static class ConsoleWorker
	{
		public static void WriteLine(string text)
		{
			Console.WriteLine(text);
		}

		public static bool AskQuestion(string question)
		{
			Console.Write($"{question} д(ДА)/н(Нет):");
			return GetAnswer();
		}

		public static void WriteError(string errorMessage)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Ошибка: {errorMessage}");
			Console.ResetColor();
		}

		public static void Debug(string text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"DEBUG {text}");
			Console.ResetColor();
		}

		public static void Spin()
		{
			while (true)
			{
				var spin = new ConsoleSpiner();
				spin.Turn();
			}
		}

		private static bool GetAnswer()
		{
			var answer = Console.ReadLine();
			return answer != null &&
					(answer.Equals("д", StringComparison.OrdinalIgnoreCase)
					|| answer.Equals("да", StringComparison.OrdinalIgnoreCase));
		}

		public static string GetLogin()
		{
			Console.Write("логин:");
			return Console.ReadLine();
		}

		public static SecureString GetPassword()
		{
			Console.Write("пароль:");
			var password = new SecureString();
			while (true)
			{
				var currentKey = Console.ReadKey(true);
				if (currentKey.Key == ConsoleKey.Enter)
					break;

				if (currentKey.Key == ConsoleKey.Backspace)
				{
					if (password.Length <= 0)
						continue;
					password.RemoveAt(password.Length - 1);
					Console.Write("\b \b");
				}
				else if (currentKey.KeyChar != '\u0000')
					// KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
				{
					password.AppendChar(currentKey.KeyChar);
					Console.Write("*");
				}
			}

			Console.WriteLine();
			return password;
		}
	}


	public class ConsoleSpiner
	{
		private int counter;

		public ConsoleSpiner()
		{
			counter = 0;
		}

		public void Turn()
		{
			counter++;
			switch (counter % 4)
			{
				case 0:
					Console.Write("/");
					break;
				case 1:
					Console.Write("-");
					break;
				case 2:
					Console.Write("\\");
					break;
				case 3:
					Console.Write("|");
					break;
			}

			Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
		}
	}
}