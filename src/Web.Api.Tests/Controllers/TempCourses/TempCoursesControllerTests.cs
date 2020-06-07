using System.IO;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Serilog;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Controllers;
using Ulearn.Web.Api.Controllers.Groups;
using Ulearn.Web.Api.Models.Parameters.Groups;

namespace Web.Api.Tests.Controllers.TempCourses
{
	[TestFixture]
	public class TempCoursesControllerTests : BaseControllerTests
	{
		private TempCourseController tempCourseController;
		private ITempCoursesRepo tempCoursesRepo;
		private ICourseRolesRepo courseRolesRepo;

		[SetUp]
		public void SetUp()
		{
			SetupTestInfrastructureAsync(services =>
			{
				services.AddScoped<TempCourseController>();
				
			}).GetAwaiter().GetResult();
			tempCourseController = GetController<TempCourseController>();
			tempCoursesRepo = serviceProvider.GetService<ITempCoursesRepo>();
			courseRolesRepo = serviceProvider.GetService<ICourseRolesRepo>();
		}

		[Test]
		public async Task CreateTempCourse()
		{
			var baseCourse = new Mock<ICourse>();
			baseCourse.Setup(c => c.Id).Returns("courseId");
			await courseRolesRepo.ToggleRoleAsync(baseCourse.Object.Id, TestUsers.User.Id, CourseRoleType.CourseAdmin, TestUsers.Admin.Id);
			await AuthenticateUserInControllerAsync(tempCourseController, TestUsers.User).ConfigureAwait(false);

			var result = await tempCourseController.CreateCourse(baseCourse.Object.Id).ConfigureAwait(false);
			Assert.AreEqual(1, result);
		}
	}
}