using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using uLearn;
using uLearn.Configuration;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Authorization;
using Ulearn.Web.Api.Controllers.Notifications;
using Ulearn.Web.Api.Models.Binders;
using Ulearn.Web.Api.Swagger;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Hosting;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging.Serilog;
using Vostok.Logging.Serilog.Enrichers;
using Vostok.Metrics;
using Web.Api.Configuration;
using ILogger = Serilog.ILogger;

namespace Ulearn.Web.Api
{
    public class WebApplication : AspNetCoreVostokApplication
    {
        protected override void OnStarted(IVostokHostingEnvironment hostingEnvironment)
        {
            hostingEnvironment.MetricScope.SystemMetrics(1.Minutes());
			
#if DEBUG
			/* Initialize EntityFramework Profiler. See https://www.hibernatingrhinos.com/products/efprof/learn for details */
			HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
#endif
        }

        protected override IWebHost BuildWebHost(IVostokHostingEnvironment hostingEnvironment)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.With<ThreadEnricher>()
                .Enrich.With<FlowContextEnricher>()
                .MinimumLevel.Information()
                .WriteTo.Airlock(LogEventLevel.Information);
			
            if (hostingEnvironment.Log != null)
                loggerConfiguration = loggerConfiguration.WriteTo.VostokLog(hostingEnvironment.Log);
            var logger = loggerConfiguration.CreateLogger();
			
			var configuration = ApplicationConfiguration.Read<WebApiConfiguration>();
			
            return new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{hostingEnvironment.Configuration["port"]}/")
                .AddVostokServices()
				.ConfigureServices(s => ConfigureServices(s, hostingEnvironment, logger, configuration))
                .UseSerilog(logger)
                .Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                    app.UseVostok();
                    if (env.IsDevelopment())
                        app.UseDeveloperExceptionPage();
					
					/* Add CORS. Should be before app.UseMvc() */
					app.UseCors(builder =>
					{
						builder
							.WithOrigins(configuration.Web.Cors.AllowOrigins)
							.AllowAnyMethod()
							.WithHeaders(new string[] { "Authorization" })
							.AllowCredentials();
					});
					
					app.UseAuthentication();
					app.UseMvc();
					
					/* Configure swagger documentation. Now it's available at /swagger/v1/swagger.json.
					 * See https://github.com/domaindrivendev/Swashbuckle.AspNetCore for details */
					app.UseSwagger(c =>
					{
						c.RouteTemplate = "documentation/{documentName}/swagger.json";
					});
					/* And add swagger UI, available at /swagger */
					app.UseSwaggerUI(c =>
					{
						c.SwaggerEndpoint("/documentation/v1/swagger.json", "Ulearn API");
						c.DocumentTitle = "UlearnApi";
						c.RoutePrefix = "documentation";
					});
					
					var database = app.ApplicationServices.GetService<UlearnDb>();
					database.MigrateToLatestVersion();
					var initialDataCreator = app.ApplicationServices.GetService<InitialDataCreator>();
					database.CreateInitialDataAsync(initialDataCreator);
                })
                .Build();
        }

		private void ConfigureServices(IServiceCollection services, IVostokHostingEnvironment hostingEnvironment, Logger logger, WebApiConfiguration configuration)
		{
			/* TODO (andgein): use UlearnDbFactory here */
			services.AddDbContextPool<UlearnDb>(
				options => options
					.UseLazyLoadingProxies()
					.UseSqlServer(hostingEnvironment.Configuration["database"])
			);
			
			services.Configure<WebApiConfiguration>(options => hostingEnvironment.Configuration.Bind(options));
			
			ConfigureDi(services, logger);
			ConfigureMvc(services);
			ConfigureSwaggerDocumentation(services);

			/* Add CORS */
			services.AddCors();

			ConfigureAuthServices(services, configuration, logger);
		}

		private static void ConfigureMvc(IServiceCollection services)
		{
			/* Asp.NET Core MVC */
			services.AddMvc(options =>
				{
					/* Add binder for passing Course object to actions */
					options.ModelBinderProviders.Insert(0, new CourseBinderProvider());

					/* Disable model checking because in other case stack overflow raised on course model binding.
					   See https://github.com/aspnet/Mvc/issues/7357 for details */
					options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Course)));
				}
			);
		}

		private static void ConfigureSwaggerDocumentation(IServiceCollection services)
		{
			/* Swagger API documentation generator. See https://github.com/domaindrivendev/Swashbuckle.AspNetCore for details */
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info
				{
					Title = "Ulearn API",
					Version = "v1",
					Description = "An API for ulearn.me",
					Contact = new Contact
					{
						Name = "Ulearn support",
						Email = "support@ulearn.me"
					}
				});
				// c.MapType<Course>(() => new Schema {  });
				c.OperationFilter<AuthResponsesOperationFilter>();
			});
		}

		private static void ConfigureDi(IServiceCollection services, Logger logger)
		{
			services.AddSingleton<ILogger>(logger);
			services.AddSingleton<InitialDataCreator>();
			services.AddSingleton<WebCourseManager>();
			services.AddScoped<UlearnUserManager>();
			services.AddScoped<IAuthorizationHandler, CourseRoleAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, CourseAccessAuthorizationHandler>();
			services.AddScoped<INotificationDataPreloader, NotificationDataPreloader>();

			/* DI for database repos */
			services.AddScoped<IUsersRepo, UsersRepo>();
			services.AddScoped<ICommentsRepo, CommentsRepo>();
			services.AddScoped<IUserRolesRepo, UserRolesRepo>();
			services.AddScoped<ICoursesRepo, CoursesRepo>();
			services.AddScoped<ISlideCheckingsRepo, SlideCheckingsRepo>();
			services.AddScoped<IGroupsRepo, GroupsRepo>();
			services.AddScoped<IGroupsCreatorAndCopier, GroupsCreatorAndCopier>();
			services.AddScoped<IGroupAccessesRepo, GroupAccessesRepo>();
			services.AddScoped<IUsersGroupsGetter, UsersGroupsGetter>();
			services.AddScoped<IUserSolutionsRepo, UserSolutionsRepo>();
			services.AddScoped<IUserQuizzesRepo, UserQuizzesRepo>();
			services.AddScoped<IVisitsRepo, VisitsRepo>();
			services.AddScoped<ITextsRepo, TextsRepo>();
			services.AddScoped<INotificationsRepo, NotificationsRepo>();
			services.AddScoped<IFeedRepo, FeedRepo>();
			services.AddScoped<ISystemAccessesRepo, SystemAccessesRepo>();
			services.AddScoped<IQuizzesRepo, QuizzesRepo>();
		}

		private void ConfigureAuthServices(IServiceCollection services, WebApiConfiguration configuration, Logger logger)
		{
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<UlearnDb>()
				.AddDefaultTokenProviders();

			/* Configure sharing cookies between application.
			   See https://docs.microsoft.com/en-us/aspnet/core/security/cookie-sharing?tabs=aspnetcore2x for details */
			services.AddDataProtection()
				.PersistKeysToFileSystem(new DirectoryInfo(configuration.Web.CookieKeyRingDirectory))
				.SetApplicationName("ulearn");

			services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.Name = configuration.Web.CookieName;
				options.Cookie.Expiration = TimeSpan.FromDays(14);
				options.Cookie.Domain = configuration.Web.CookieDomain;
				options.LoginPath = "/users/login";
				options.LogoutPath = "/users/logout";
				options.Events.OnRedirectToLogin = context =>
				{
					/* Replace standard redirecting to LoginPath */
					context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					return Task.CompletedTask;
				};
				options.Events.OnRedirectToAccessDenied = context =>
				{
					/* Replace standard redirecting to AccessDenied */
					context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
					return Task.CompletedTask;
				};
			});

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					
					ValidIssuer = configuration.Web.Authentication.Jwt.Issuer,
					ValidAudience = configuration.Web.Authentication.Jwt.Audience,
					IssuerSigningKey = JwtBearerHelpers.CreateSymmetricSecurityKey(configuration.Web.Authentication.Jwt.IssuerSigningKey)
				};
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("Instructors", policy => policy.Requirements.Add(new CourseRoleRequirement(CourseRole.Instructor)));
				options.AddPolicy("CourseAdmins", policy => policy.Requirements.Add(new CourseRoleRequirement(CourseRole.CourseAdmin)));
				options.AddPolicy("SysAdmins", policy => policy.RequireRole(new List<string> { LmsRoles.SysAdmin.GetDisplayName() }));

				foreach (var courseAccessType in Enum.GetValues(typeof(CourseAccessType)).Cast<CourseAccessType>())
				{
					var policyName = courseAccessType.GetAuthorizationPolicyName();
					options.AddPolicy(policyName, policy => policy.Requirements.Add(new CourseAccessRequirement(courseAccessType)));
				}
			});
		}
	}
}