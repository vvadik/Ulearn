using System.Threading.Tasks;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Core.Courses;

namespace Ulearn.Web.Api.Models
{
	public class ApiCourseManager : ICourseUpdater
	{
		private readonly IServiceScopeFactory serviceScopeFactory;
		
		public ApiCourseManager(IServiceScopeFactory serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
		} 

		public async Task UpdateCourses()
		{
			throw new System.NotImplementedException();
		}

		public async Task UpdateTempCourses()
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var tempCoursesRepo = (TempCoursesRepo)scope.ServiceProvider.GetService(typeof(ITempCoursesRepo));
				var tempCourses = await tempCoursesRepo!.GetTempCoursesAsync();
			}
		}
	}
}