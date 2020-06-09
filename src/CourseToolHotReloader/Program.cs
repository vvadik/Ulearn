using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.DirectoryWorkers;
using CourseToolHotReloader.Exceptions;
using CourseToolHotReloader.Log;
using CourseToolHotReloader.LoginAgent;
using ErrorType = CourseToolHotReloader.Dtos.ErrorType;

namespace CourseToolHotReloader
{
	internal class Program
	{
		private static IContainer container;
		private static IConfig config;
		private static IUlearnApiClient ulearnApiClient;

		private static void Main()
		{
			Init();

			try
			{
				Startup().Wait();
			}
			catch (AggregateException e)
			{
				switch (e.InnerExceptions.Single())
				{
					case UriFormatException _:
						ConsoleWorker.WriteError("Указанный Base Url недоступен");
						break;
					case IOException _:
						ConsoleWorker.WriteError("Вероятно был добавлен файл слишком большого размера");
						break;
					case UnauthorizedException _:
					case ForbiddenException _:
						ConsoleWorker.WriteError(e.Message);
						break;
					default:
						Logger.Log.Error(e);
						break;
				}
			}
			catch (Exception e)
			{
				Logger.Log.Error(e);
				ConsoleWorker.WriteError("Неизвестная ошибка. Подробнее в логах");
			}
		}

		private static void Init()
		{
			Logger.InitLogger();
			container = ConfigureAutofac.Build();

			config = container.Resolve<IConfig>();
			ulearnApiClient = container.Resolve<IUlearnApiClient>();
		}

		private static async Task Startup()
		{
			await Login();

			if (!await TempCourseExist())
				await CreateCourse();

			await SendFullCourse();

			StartWatch();
		}

		private static void StartWatch()
		{
			var courseWatcher = container.Resolve<ICourseWatcher>();
			courseWatcher.StartWatch();
		}

		private static async Task Login()
		{
			var isLoginSuccess = await container.Resolve<ILoginAgent>().SignIn();

			if (isLoginSuccess)
			{
				ConsoleWorker.WriteLine("Авторизация прошла успешно");
			}
			else
			{
				ConsoleWorker.WriteError("Ошибка авторизации");
			}
		}

		private static async Task SendFullCourse()
		{
			var tempCourseUpdateResponse = await ulearnApiClient.SendFullCourse(config.Path, config.CourseId);
			if (tempCourseUpdateResponse.ErrorType == ErrorType.NoErrors)
				ConsoleWorker.WriteLine("Первоначальная полная загрузка курса прошла успешно");
			else
				throw new Exception(tempCourseUpdateResponse.Message);
		}

		private static async Task<bool> TempCourseExist()
		{
			var hasTempCourseResponse = await ulearnApiClient.HasCourse(config.CourseId);

			if (string.IsNullOrEmpty(hasTempCourseResponse?.MainCourseId))
				ConsoleWorker.WriteError($"Курс с CourseId = \"{config.CourseId}\" не найден проверьте на опечатки или обратитесь к админу ulearn");

			if (hasTempCourseResponse.HasTempCourse)
				ConsoleWorker.WriteLine($"Обнаружен существующий временный курс с id {hasTempCourseResponse.TempCourseId}");

			return hasTempCourseResponse.HasTempCourse;
		}

		private static async Task CreateCourse()
		{
			var createResponse = await ulearnApiClient.CreateCourse(config.CourseId);
			if (createResponse.ErrorType == ErrorType.NoErrors)
				ConsoleWorker.WriteLine(createResponse.Message);
			else
				throw new Exception(createResponse.Message);
		}
	}
}