using System;
using System.IO;
using System.Net;
using System.Reflection;
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
		private readonly string path;

		public Authorizer(IConfig config)
		{
			this.config = config;
			path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\config.txt";
		}

		public async Task SignIn()
		{
			var loginPasswordParameters = GetLoginPasswordParameters();
			var configExist = File.Exists(path);

			try
			{
				config.JwtToken = await HttpMethods.GetJwtToken(loginPasswordParameters);
			}
			catch (Exception e)
			{
				if (configExist)
					File.Delete(path);

				Console.WriteLine(e);
				throw;
			}

			if (!configExist)
				SavePassword(loginPasswordParameters);
		}

		private LoginPasswordParameters GetLoginPasswordParameters()
		{
			if (File.Exists(path))
			{
				var loginPasswordParameters = ReadFile();
				Console.Write($"Войти под {loginPasswordParameters.Login}? д(ДА)/н(Нет):");
				if (GetAnswer())
					return loginPasswordParameters;
			}

			return new LoginPasswordParameters
			{
				Login = GetLogin(),
				Password = new NetworkCredential(string.Empty, GetPassword()).Password
			};
		}

		private static SecureString GetPassword()
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

		private static string GetLogin()
		{
			Console.Write("логин:");
			return Console.ReadLine();
		}

		private void SavePassword(LoginPasswordParameters loginPasswordParameters)
		{
			Console.Write("Запомнить меня? д(ДА)/н(Нет):");

			if (!GetAnswer())
				return;

			Console.WriteLine("Мы сохранили ваш логин и пароль в файле:");
			Console.WriteLine(path);
			CreateFile(loginPasswordParameters);
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

		private static bool GetAnswer()
		{
			var answer = Console.ReadLine();
			return answer != null &&
					(answer.Equals("д", StringComparison.OrdinalIgnoreCase)
					|| answer.Equals("да", StringComparison.OrdinalIgnoreCase));
		}
	}
}