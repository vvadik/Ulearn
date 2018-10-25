using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Community.AspNetCore.ExceptionHandling;
using Community.AspNetCore.ExceptionHandling.Mvc;
using Database;
using Database.Di;
using Database.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using uLearn;
using uLearn.Configuration;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Authorization;
using Ulearn.Web.Api.Controllers;
using Ulearn.Web.Api.Controllers.Notifications;
using Ulearn.Web.Api.Models.Binders;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Swagger;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Hosting;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging.Serilog;
using Vostok.Logging.Serilog.Enrichers;
using Vostok.Metrics;
using Vostok.Tracing;
using Web.Api.Configuration;
using Enum = System.Enum;
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
							.WithHeaders("Authorization")
							.WithExposedHeaders("Location")
							.AllowCredentials();
					});
					
					/* Add exception handling policy.
					   See https://github.com/IharYakimush/asp-net-core-exception-handling */
					app.UseExceptionHandlingPolicies();
					
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

		private void ConfigureServices(IServiceCollection services, IVostokHostingEnvironment hostingEnvironment, ILogger logger, WebApiConfiguration configuration)
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
			ConfigureExceptionPolicy(services, logger);

			/* Add CORS */
			services.AddCors();

			ConfigureAuthServices(services, configuration);
		}

		public static void ConfigureMvc(IServiceCollection services)
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
				
				/* See https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters#installation for manual about swagger request and response examples */
				c.ExampleFilters();
				
				c.OperationFilter<BadRequestResponseOperationFilter>();
				c.OperationFilter<AuthResponsesOperationFilter>();
				c.OperationFilter<RemoveCourseParameterOperationFilter>();
				
				c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
				
				c.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					In = "header",
					Description = "Please insert JWT with Bearer into field. Example: \"Bearer {token}\"",
					Name = "Authorization",
					Type = "apiKey"
				});
				c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
				{
					{ "Bearer", new string [] {} }
				});
				
				/* See https://docs.microsoft.com/ru-ru/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio%2Cvisual-studio-xml for details */ 
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
			
			services.AddSwaggerExamplesFromAssemblyOf<ApiResponse>();
		}
		
		private void ConfigureExceptionPolicy(IServiceCollection services, ILogger logger)
		{
			/* See https://github.com/IharYakimush/asp-net-core-exception-handling for details */
			services.AddExceptionHandlingPolicies(options =>
			{
				options.For<StatusCodeException>()
					.Response(exception => exception.Code)
					.WithObjectResult((r, exception) => new ErrorResponse(exception.Message))
					.Handled();
				
				/* Ensure that all exception types are handled by adding handler for generic exception at the end. */
				options.For<Exception>()
					.Log(lo =>
					{
						lo.Level = (context, exception) => LogLevel.Error;
					})
					.Response(exception => (int) HttpStatusCode.InternalServerError, ResponseAlreadyStartedBehaviour.GoToNextHandler)
					.ClearCacheHeaders()
					.WithObjectResult((r, exception) => new ErrorResponse("Internal error occured"
#if DEBUG
	+ $". {exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace}"
#endif
						))
					.Handled();
			});
		}		

		public static void ConfigureDi(IServiceCollection services, ILogger logger)
		{
			services.AddSingleton(logger);
			services.AddScoped<IAuthorizationHandler, CourseRoleAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, CourseAccessAuthorizationHandler>();
			services.AddScoped<INotificationDataPreloader, NotificationDataPreloader>();

			services.AddDatabaseServices(logger);
		}

		public static void ConfigureAuthServices(IServiceCollection services, WebApiConfiguration configuration)
		{
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
				options.AddPolicy("Instructors", policy => policy.Requirements.Add(new CourseRoleRequirement(CourseRoleType.Instructor)));
				options.AddPolicy("CourseAdmins", policy => policy.Requirements.Add(new CourseRoleRequirement(CourseRoleType.CourseAdmin)));
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