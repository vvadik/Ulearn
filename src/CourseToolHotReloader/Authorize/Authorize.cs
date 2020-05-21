using System;
using System.Security;

namespace CourseToolHotReloader.Authorize
{
	public class Authorize
	{
		public SecureString GetPassword()
		{
			var pwd = new SecureString();
			while (true)
			{
				var i = Console.ReadKey(true);
				if (i.Key == ConsoleKey.Enter)
				{
					break;
				}

				if (i.Key == ConsoleKey.Backspace)
				{
					if (pwd.Length <= 0)
						continue;
					pwd.RemoveAt(pwd.Length - 1);
					Console.Write("\b \b");
				}
				else if (i.KeyChar != '\u0000' ) // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
				{
					pwd.AppendChar(i.KeyChar);
					Console.Write("*");
				}
			}
			return pwd;
		}
	}
}