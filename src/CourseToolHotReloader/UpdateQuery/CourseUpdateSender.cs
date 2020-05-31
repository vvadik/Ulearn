using System;
using System.Threading.Tasks;
using CourseToolHotReloader.ApiClient;
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

			var courseDelivered = await ulearnApiClient.TrySendCourseUpdates(courseUpdateQuery.GetAllCourseUpdate(),
				courseUpdateQuery.GetAllDeletedFiles(), config.JwtToken.Token, config.CourseId);

			cs.Stop();

			if (courseDelivered)
			{
				courseUpdateQuery.Clear();
			}
		}

		public async Task SendFullCourse()
		{
			var cs = ConsoleSpinner.CreateAndRunWithText("Загружаем курс на ulearn");

			await ulearnApiClient.SendFullCourse(config.Path, config.JwtToken.Token, config.CourseId);

			cs.Stop();
		}
	}
}