using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.Log;

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
				if (e is HttpRequestException)
				{
					ConsoleWorker.WriteError("Отсутствует соединение с сервером ulearn");
					Environment.Exit(1);
				}

				if (configExist)
					File.Delete(path);

				ConsoleWorker.WriteLine(e.Message); //todo
				Environment.Exit(1);
			}

			if (!configExist)
				SavePassword(loginPasswordParameters);
			
			ConsoleWorker.WriteLine("Авторизация прошла успешно!");
		}

		private LoginPasswordParameters GetLoginPasswordParameters()
		{
			if (File.Exists(path))
			{
				var loginPasswordParameters = ReadFile();

				if (ConsoleWorker.AskQuestion($"Войти под {loginPasswordParameters.Login}?"))
					return loginPasswordParameters;
			}

			return new LoginPasswordParameters
			{
				Login = ConsoleWorker.GetLogin(),
				Password = new NetworkCredential(string.Empty, ConsoleWorker.GetPassword()).Password
			};
		}


		private void SavePassword(LoginPasswordParameters loginPasswordParameters)
		{
			if (!ConsoleWorker.AskQuestion("Запомнить меня?"))
				return;

			ConsoleWorker.WriteLine("Мы сохранили ваш логин и пароль в файле:");
			ConsoleWorker.WriteLine(path);
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
				ConsoleWorker.WriteLine(ex.ToString());
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