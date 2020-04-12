using System.IO;
using CourseToolHotReloader.ApiClient;

namespace CourseToolHotReloader.UpdateQuery
{
	public interface ICourseUpdateSender
	{
		void SendCourseUpdates();
	}

	public class CourseUpdateSender : ICourseUpdateSender
	{
		private readonly ICourseUpdateQuery _courseUpdateQuery;
		private readonly IUlearnApiClient _ulearnApiClient;

		public CourseUpdateSender(ICourseUpdateQuery courseUpdateQuery, IUlearnApiClient ulearnApiClient)
		{
			_courseUpdateQuery = courseUpdateQuery;
			_ulearnApiClient = ulearnApiClient;
		}

		public void SendCourseUpdates()
		{
			_ulearnApiClient.SendCourseUpdates(_courseUpdateQuery.GetAllCourseUpdate(), _courseUpdateQuery.GetAllDeletedFiles());
		}
	}
}