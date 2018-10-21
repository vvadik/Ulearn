using Database.Models;
using Database.Repos;
using Database.Repos.Comments;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Database.Di
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDatabaseServices(this IServiceCollection services, ILogger logger)
		{
			services.AddSingleton<InitialDataCreator>();
			
			var courseManager = new WebCourseManager(logger);
			services.AddSingleton<WebCourseManager>(courseManager);
			services.AddSingleton<IWebCourseManager>(courseManager);
			
			services.AddScoped<UlearnUserManager>();
			
			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
				{
					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 6;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
				})
				.AddEntityFrameworkStores<UlearnDb>()
				.AddDefaultTokenProviders();
			
			/* DI for database repos: */
			
			/* Groups */
			services.AddScoped<IGroupsRepo, GroupsRepo>();
			services.AddScoped<IGroupMembersRepo, GroupMembersRepo>();
			services.AddScoped<IGroupAccessesRepo, GroupAccessesRepo>();
			services.AddScoped<IManualCheckingsForOldSolutionsAdder, ManualCheckingsForOldSolutionsAdder>();
			services.AddScoped<IGroupsCreatorAndCopier, GroupsCreatorAndCopier>();
			services.AddScoped<IUsersGroupsGetter, UsersGroupsGetter>();
			
			/* Comments */
			services.AddScoped<ICommentsRepo, CommentsRepo>();
			services.AddScoped<ICommentLikesRepo, CommentLikesRepo>();
			services.AddScoped<ICommentPoliciesRepo, CommentPoliciesRepo>();
			
			/* Others */
			services.AddScoped<IUsersRepo, UsersRepo>();
			services.AddScoped<IUserRolesRepo, UserRolesRepo>();
			services.AddScoped<ICoursesRepo, CoursesRepo>();
			services.AddScoped<ISlideCheckingsRepo, SlideCheckingsRepo>();
			services.AddScoped<IUserSolutionsRepo, UserSolutionsRepo>();
			services.AddScoped<IUserQuizzesRepo, UserQuizzesRepo>();
			services.AddScoped<IVisitsRepo, VisitsRepo>();
			services.AddScoped<ITextsRepo, TextsRepo>();
			services.AddScoped<INotificationsRepo, NotificationsRepo>();
			services.AddScoped<IFeedRepo, FeedRepo>();
			services.AddScoped<ISystemAccessesRepo, SystemAccessesRepo>();
			services.AddScoped<IQuizzesRepo, QuizzesRepo>();

			return services;
		}
	}
}