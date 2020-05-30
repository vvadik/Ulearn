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
			var ct = ConsoleWorker.Spin();
			
			var courseDelivered = await ulearnApiClient.TrySendCourseUpdates(courseUpdateQuery.GetAllCourseUpdate(),
				courseUpdateQuery.GetAllDeletedFiles(), config.JwtToken.Token, config.CourseId);

			ct.Cancel();
			
			if (courseDelivered)
			{
				courseUpdateQuery.Clear();
			}
		}

		public async Task SendFullCourse()
		{
			await ulearnApiClient.SendFullCourse(config.Path, config.JwtToken.Token, config.CourseId);
		}
	}
}