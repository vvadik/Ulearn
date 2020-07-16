using System;
using System.IO;
using System.Linq;
using System.Net.Http;
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
				Logger.Log.Error(e);
				switch (e.InnerExceptions.Single())
				{
					case InvalidOperationException _:
					case UriFormatException _:
					case ArgumentException _:
						ConsoleWorker.WriteError("Указанный в config.json baseUrl недоступен");
						break;
					case IOException _:
						ConsoleWorker.WriteError("Ошибка ввода-вывода. Подробнее в логах");
						break;
					case UnauthorizedException _:
					case ForbiddenException _:
					case HttpRequestException _:
					case CourseLoadingException _:
						ConsoleWorker.WriteError(e.Message);
						break;
					default:
						ConsoleWorker.WriteError("Ошибка. Подробнее в логах");
						break;
				}
			}
			catch (Exception e)
			{
				Logger.Log.Error(e);
				ConsoleWorker.WriteError("Ошибка. Подробнее в логах");
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
			var userId = await Login();
			if (userId == null)
				return;

			if (!CourseIdCorrect())
				return;

			if (!await TempCourseExist(userId))
				if (!await CreateCourse())
					return;

			await SendFullCourse();

			StartWatch();
		}

		private static void StartWatch()
		{
			var courseWatcher = container.Resolve<ICourseWatcher>();
			courseWatcher.StartWatch();
		}

		private static async Task<string> Login()
		{
			var userId = await container.Resolve<ILoginAgent>().SignIn();

			if (userId != null)
				ConsoleWorker.WriteLine("Авторизация прошла успешно");
			else
				ConsoleWorker.WriteError("Ошибка авторизации");

			return userId;
		}

		private static async Task SendFullCourse()
		{
			var tempCourseUpdateResponse = await ulearnApiClient.SendFullCourse(config.Path, config.CourseId);
			if (tempCourseUpdateResponse.ErrorType == ErrorType.NoErrors)
				ConsoleWorker.WriteLine("Первоначальная полная загрузка курса прошла успешно");
			else
			{
				throw new CourseLoadingException(tempCourseUpdateResponse.Message);
			}
		}

		private static bool CourseIdCorrect()
		{
			if (!config.CourseIds.TryGetValue(config.Path, out var courseId) || string.IsNullOrEmpty(courseId))
			{
				ConsoleWorker.WriteError($"CourseId для \"{config.Path}\" не задан в config.json.\r\n"
					+ @"Нужно указать в формате ""сourseIds"": {""C:\\Курсы\\курс1"" : ""courseId1"", ""C:\\Курсы\\курс2"" : ""courseId2""}"
					+ $"\r\nconfig.json находится \"{config.PathToConfigFile}\"");
				return false;
			}

			return true;
		}

		private static async Task<bool> TempCourseExist(string userId)
		{
			var tempCourseId = GetTmpCourseId(config.CourseId, userId);
			var hasTempCourse = await ulearnApiClient.HasCourse(tempCourseId);

			if (hasTempCourse)
				ConsoleWorker.WriteLine($"Обнаружен существующий временный курс с id {tempCourseId}");

			return hasTempCourse;
		}

		private static string GetTmpCourseId(string baseCourseId, string userId)
		{
			return $"{baseCourseId}_{userId}";
		}

		private static async Task<bool> CreateCourse()
		{
			var createResponse = await ulearnApiClient.CreateCourse(config.CourseId);
			if (createResponse.ErrorType != ErrorType.NoErrors)
				ConsoleWorker.WriteError(createResponse.Message);
			else
				ConsoleWorker.WriteLine(createResponse.Message);

			return createResponse.ErrorType == ErrorType.NoErrors;
		}

		public class CourseLoadingException : Exception
		{
			public CourseLoadingException(string message)
				: base(message)
			{
			}

			public CourseLoadingException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
		}
	}
}