using System.IO;
using System.Threading.Tasks;
using CourseToolHotReloader.ApiClient;

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
			await ulearnApiClient.SendCourseUpdates(courseUpdateQuery.GetAllCourseUpdate(), courseUpdateQuery.GetAllDeletedFiles(), config.JwtToken.Token, config.CourseId);
		}

		public async Task SendFullCourse()
		{
			await ulearnApiClient.SendFullCourse(config.Path, config.JwtToken.Token, config.CourseId);
		}
	}
}