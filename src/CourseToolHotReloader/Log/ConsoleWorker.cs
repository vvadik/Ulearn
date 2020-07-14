using System;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace CourseToolHotReloader.Log
{
	public static class ConsoleWorker
	{
		public static void WriteLine(string text)
		{
			Console.WriteLine(text);
			Logger.Log.Info(text);
		}

		public static bool AskQuestion(string question)
		{
			Console.Write($"{question} д(ДА)/н(Нет):");
			return GetAnswer();
		}

		public static void WriteError(string errorMessage)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			var text = errorMessage;
			Console.WriteLine(text);
			Console.ResetColor();
			Logger.Log.Info(text);
		}

		public static void WriteAlert(string alertMessage)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			var text = alertMessage;
			Console.WriteLine(text);
			Console.ResetColor();
			Logger.Log.Info(text);
		}

		public static void Debug(string text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"DEBUG {text}");
			Console.ResetColor();
			Logger.Log.Debug(text);
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


	class ConsoleSpinner
	{
		private int counter;
		private readonly string[] sequence;
		private readonly string space;
		private CancellationTokenSource cancelTokenSource;

		private ConsoleSpinner()
		{
			counter = 0;
			sequence = new[] { " /", " -", " \\", " |" };
			//sequence = new[] { "░", "▒", "▓" };
			//sequence = new[] { "╚═", "═╝", "═╗", "╔═" };
			//sequence = new[] { "█", "▄" };
			//sequence = new[] { ".", "o", "0", "o"};
			//sequence = new[] { "+", "x", "  +", "  x", "   +", "   x", "    +", "    x", "     +", "     x"  };
			//sequence = new[] { "█        ", "██       ", "███      ", "████     ", "█████    ", "██████   ", "███████  ", "████████ ", "█████████", "████████ ", "███████  ", "██████   ", "█████    ", "███      ", "██       " };
			//sequence = new[] { "█        ", " █       ", "  █      ", "   █     ", "    █    ", "     █   ", "      █  ", "       █ ", "        █", "       █ ", "      █  ", "     █   ", "    █    ", "  █      ", " █       " };
			//sequence = new[] { "V", "<", "^", ">" };
			//sequence = new[] { ".   ", "..  ", "... ", "...." };
			space = "  ";                
		}

		public static ConsoleSpinner CreateAndRunWithText(string text)
		{
			Console.Write(text);
			var cs = new ConsoleSpinner();
			cs.Start();
			return cs;
		}

		private void Start()
		{
			cancelTokenSource = new CancellationTokenSource();
			var ct = cancelTokenSource.Token;
			Task.Run(async () =>
			{
				Console.CursorVisible = false;
				while (true)
				{
					await Task.Delay(100);

					if (ct.IsCancellationRequested)
					{
						Console.CursorVisible = true;
						return;
					}

					Turn();
				}
			}, cancelTokenSource.Token);
		}

		public void Stop()
		{
			cancelTokenSource.Cancel();
			cancelTokenSource.Dispose();
			Console.Write(space);
			Console.WriteLine();
		}

		private void Turn()
		{
			counter++;

			if (counter >= sequence.Length)
				counter = 0;

			Console.Write(sequence[counter]);
			Console.SetCursorPosition(Console.CursorLeft - sequence[counter].Length, Console.CursorTop);
		}
	}
}