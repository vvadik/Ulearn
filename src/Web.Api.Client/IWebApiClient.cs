using System.Threading.Tasks;
using Vostok.Clusterclient.Core.Model;

namespace Web.Api.Client
{
	public interface IWebApiClient
	{
		public Task<Response> GetCourseStaticFile(string courseId, string filePathRelativeToCourse);
	}
}