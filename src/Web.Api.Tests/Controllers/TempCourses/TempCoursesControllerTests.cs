using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Controllers;
using Ulearn.Web.Api.Models.Responses.TempCourses;

namespace Web.Api.Tests.Controllers.TempCourses
{
	[TestFixture]
	public class TempCoursesControllerTests : BaseControllerTests
	{
		private TempCourseController tempCourseController;
		private ITempCoursesRepo tempCoursesRepo;
		private ICourseRolesRepo courseRolesRepo;

		[OneTimeSetUp]
		public void SetUp()
		{
			SetupTestInfrastructureAsync(services => { services.AddScoped<TempCourseController>(); }).GetAwaiter().GetResult();
			tempCourseController = GetController<TempCourseController>();
			tempCoursesRepo = serviceProvider.GetService<ITempCoursesRepo>();
			courseRolesRepo = serviceProvider.GetService<ICourseRolesRepo>();
		}

		[Test]
		public async Task Create_ShouldSucceed_With_MainScenario()
		{
			var baseCourse = new Mock<ICourse>();
			baseCourse.Setup(c => c.Id).Returns("mainScenario");
			await courseRolesRepo.ToggleRoleAsync(baseCourse.Object.Id, TestUsers.User.Id, CourseRoleType.CourseAdmin, TestUsers.Admin.Id);
			await AuthenticateUserInControllerAsync(tempCourseController, TestUsers.User).ConfigureAwait(false);
			var result = await tempCourseController.CreateCourse(baseCourse.Object.Id).ConfigureAwait(false);
			Assert.AreEqual(ErrorType.NoErrors, result.ErrorType);
			var tempCourseEntity = tempCoursesRepo.Find(baseCourse.Object.Id + TestUsers.User.Id);
			Assert.NotNull(tempCourseEntity);
		}

		[Test]
		public async Task Create_ShouldReturnConflict_WhenCourseAlreadyExists()
		{
			var baseCourse = new Mock<ICourse>();
			baseCourse.Setup(c => c.Id).Returns("conflictScenario");
			await courseRolesRepo.ToggleRoleAsync(baseCourse.Object.Id, TestUsers.User.Id, CourseRoleType.CourseAdmin, TestUsers.Admin.Id);
			await AuthenticateUserInControllerAsync(tempCourseController, TestUsers.User).ConfigureAwait(false);
			await tempCourseController.CreateCourse(baseCourse.Object.Id).ConfigureAwait(false);
			var result = await tempCourseController.CreateCourse(baseCourse.Object.Id).ConfigureAwait(false);
			Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
		}

		[Test]
		public async Task Create_ShouldReturnForbidden_WhenUserAccessIsLowerThanCourseAdmin()
		{
			var baseCourse = new Mock<ICourse>();
			baseCourse.Setup(c => c.Id).Returns("forbiddenScenario");
			await AuthenticateUserInControllerAsync(tempCourseController, TestUsers.User).ConfigureAwait(false);
			await tempCourseController.CreateCourse(baseCourse.Object.Id).ConfigureAwait(false);
			var result = await tempCourseController.CreateCourse(baseCourse.Object.Id).ConfigureAwait(false);
			Assert.AreEqual(ErrorType.Forbidden, result.ErrorType);
		}
	}
}