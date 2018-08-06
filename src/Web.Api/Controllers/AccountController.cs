using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Extensions;
using Database.Models;
using Database.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Authorization;
using Ulearn.Web.Api.Models.Responses.Account;
using Web.Api.Configuration;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/account")]
	public class AccountController : BaseController
	{
		private readonly UlearnUserManager userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserRolesRepo userRolesRepo;
		private readonly CoursesRepo coursesRepo;
		private readonly WebApiConfiguration configuration;

		public AccountController(
			ILogger logger, IOptions<WebApiConfiguration> options, WebCourseManager courseManager, UlearnDb db, UlearnUserManager userManager, SignInManager<ApplicationUser> signInManager,
			UserRolesRepo userRolesRepo, CoursesRepo coursesRepo
		)
			: base(logger, courseManager, db)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.userRolesRepo = userRolesRepo;
			this.coursesRepo = coursesRepo;
			this.configuration = options.Value;
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Me()
		{
			var userId = User.GetUserId();
			var user = await userManager.FindByIdAsync(userId);
			return Json(new GetMeResponse
			{
				IsAuthenticated = true,
				User = BuildShortUserInfo(user, discloseLogin: true),
				AccountProblems = await GetAccountProblems(user).ConfigureAwait(false),
			});
		}

		private async Task<List<AccountProblem>> GetAccountProblems(ApplicationUser user)
		{
			var problems = new List<AccountProblem>();
			if (string.IsNullOrEmpty(user.Email))
				problems.Add(new AccountProblem(
					"Не указана эл. почта",
					"Укажите в профиле электронную почту, чтобы получать уведомления и восстановить доступ в случае утери пароля"
				));
			if (!string.IsNullOrEmpty(user.Email)  && ! user.EmailConfirmed)
				problems.Add(new AccountProblem(
					"Эл. почта не подтверждена",
					"Подтвердите в профиле электронную почту, чтобы получать уведомления и восстановить доступ в случае утери пароля"
				));
			
			var isInstructor = await userRolesRepo.HasUserAccessToAnyCourseAsync(user.Id, CourseRole.Instructor).ConfigureAwait(false);
			if (isInstructor && (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName)))
				problems.Add(new AccountProblem(
					"Не указаны имя или фамилия",
					"Укажите в профиле имя и фамилию, чтобы студентам было проще с вами работать"
				));
			
			return problems;
		}

		[HttpPost("token")]
		[Authorize(AuthenticationSchemes = "Identity.Application" /* = IdentityConstants.ApplicationScheme */)]
		public IActionResult Token()
		{
			var claims = User.Claims;
			
			var key = JwtBearerHelpers.CreateSymmetricSecurityKey(configuration.Web.Authentication.Jwt.IssuerSigningKey);
			var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddHours(configuration.Web.Authentication.Jwt.LifeTimeHours);

			var token = new JwtSecurityToken(
				configuration.Web.Authentication.Jwt.Issuer,
				configuration.Web.Authentication.Jwt.Audience,
				claims,
				expires: expires,
				signingCredentials: signingCredentials
			);
			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
			return Json(new TokenResponse
			{
				Token = tokenString,
			});
		}

		[HttpGet("roles")]
		[Authorize]
		public async Task<IActionResult> CourseRoles()
		{
			var userId = User.GetUserId();	
			var isSystemAdministrator = User.IsSystemAdministrator();
			
			var rolesByCourse = await userRolesRepo.GetRolesAsync(userId).ConfigureAwait(false);
			var courseAccesses = await coursesRepo.GetUserAccessesAsync(userId).ConfigureAwait(false);
			var courseAccessesByCourseId = courseAccesses.GroupBy(a => a.CourseId).Select(
				g => new CourseAccessResponse
				{
					CourseId = g.Key,
					Accesses = g.Select(a => a.AccessType).ToList()
				}
			).ToList();
			
			return Json(new CourseRolesResponse
			{
				IsSystemAdministrator = isSystemAdministrator,
				Roles = rolesByCourse.Where(kvp => kvp.Value != CourseRole.Student).Select(kvp => new CourseRoleResponse
				{
					CourseId = kvp.Key,
					Role = kvp.Value,
				}).ToList(),
				Accesses = courseAccessesByCourseId,
			});
		}

		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync().ConfigureAwait(false);
			
			return Json(new LogoutResponse
			{
				Logout = true
			});
		}
	}
}