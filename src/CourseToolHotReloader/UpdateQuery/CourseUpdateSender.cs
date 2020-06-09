using System;
using System.Threading.Tasks;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.Dtos;
using CourseToolHotReloader.Log;

namespace CourseToolHotReloader.UpdateQuery
{
	public interface ICourseUpdateSender
	{
		Task SendCourseUpdates();
		Task SendFullCourse();
	}

	public class CourseUpdateSender : ICourseUpdateSender
	{
		private readonly ICourseUpdateQuery courseUpdateQuery;
		private readonly IUlearnApiClient ulearnApiClient;
		private readonly IConfig config;

		public CourseUpdateSender(ICourseUpdateQuery courseUpdateQuery, IUlearnApiClient ulearnApiClient, IConfig config)
		{
			this.courseUpdateQuery = courseUpdateQuery;
			this.ulearnApiClient = ulearnApiClient;
			this.config = config;
		}

		public async Task SendCourseUpdates()
		{
			var cs = ConsoleSpinner.CreateAndRunWithText("Загружаем изменения на ulearn");

			var errors = await ulearnApiClient.SendCourseUpdates(courseUpdateQuery.GetAllCourseUpdate(),
				courseUpdateQuery.GetAllDeletedFiles(), config.CourseId);

			cs.Stop();

			if (errors.ErrorType != ErrorType.NoErrors)
				ConsoleWorker.WriteError(errors.Message);
			else
				ConsoleWorker.WriteLine($"Изменения были загруженны без ошибок {DateTime.Now: hh:mm}");

			courseUpdateQuery.Clear();
		}

		public async Task SendFullCourse()
		{
			var cs = ConsoleSpinner.CreateAndRunWithText("Загружаем курс на ulearn");

			var errors = await ulearnApiClient.SendFullCourse(config.Path, config.CourseId);

			cs.Stop();

			if (errors.ErrorType != ErrorType.NoErrors)
				ConsoleWorker.WriteError(errors.Message);
			else
				ConsoleWorker.WriteLine("Курс был загружен без ошибок");
		}
	}
}