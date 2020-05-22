using System;
using System.IO;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.Dtos;

namespace CourseToolHotReloader.Authorizer
{
	public interface IAuthorizer
	{
		Task SignIn();
	}

	public class Authorizer : IAuthorizer
	{
		private readonly IConfig config;
		private const string path = @"c:\temp\config.txt";

		public Authorizer(IConfig config)
		{
			this.config = config;
		}

		public async Task SignIn()
		{
			LoginPasswordParameters loginPasswordParameters;
			var configExist = File.Exists(path);

			if (File.Exists(path))
			{
				loginPasswordParameters = ReadFile();
			}
			else
			{
				loginPasswordParameters = new LoginPasswordParameters
				{
					Login = GetLogin(),
					Password = new System.Net.NetworkCredential(string.Empty, GetPassword()).Password
				};
			}

			try
			{
				config.JwtToken = await HttpMethods.GetJwtToken(loginPasswordParameters);
			}
			catch (Exception e)
			{
				if (configExist)
				{
					File.Delete(path);
				}

				Console.WriteLine(e);
				throw;
			}

			if (!configExist)
				SavePassword(loginPasswordParameters);
		}

		private SecureString GetPassword()
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

		private string GetLogin()
		{
			Console.Write("логин:");
			return Console.ReadLine();
		}

		private void SavePassword(LoginPasswordParameters loginPasswordParameters)
		{
			Console.Write("Запомнить меня? д(ДА)/н(Нет):");
			var answer = Console.ReadLine();
			if (answer != null &&
				(answer.Equals("д", StringComparison.OrdinalIgnoreCase)
				|| answer.Equals("да", StringComparison.OrdinalIgnoreCase)))
			{
				CreateFile(loginPasswordParameters);
			}
		}

		private void CreateFile(LoginPasswordParameters loginPasswordParameters)
		{
			try
			{
				using var fileStream = File.Create(path);
				var info = new UTF8Encoding(true).GetBytes($"{loginPasswordParameters.Login}\r\n{loginPasswordParameters.Password}");
				fileStream.Write(info, 0, info.Length);
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private LoginPasswordParameters ReadFile()
		{
			using var streamReader = File.OpenText(path);

			return new LoginPasswordParameters
			{
				Login = streamReader.ReadLine(),
				Password = streamReader.ReadLine()
			};
		}
	}
}