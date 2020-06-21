using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Ulearn.Common.Api;
using Ulearn.Core.Configuration;
using Vostok.Hosting;
using Vostok.Hosting.Abstractions;

namespace AntiPlagiarism.Web
{
	public class WebApplication : BaseApiWebApplication
	{
		private AddNewSubmissionWorker addNewSubmissionWorker; // держит ссылку на воркеры
		
		protected override IApplicationBuilder ConfigureWebApplication(IApplicationBuilder app)
		{
			var database = app.ApplicationServices.GetService<AntiPlagiarismDb>();
			database.MigrateToLatestVersion();
			addNewSubmissionWorker = app.ApplicationServices.GetService<AddNewSubmissionWorker>();
			return app;
		}

		protected override void ConfigureServices(IServiceCollection services, IVostokHostingEnvironment hostingEnvironment, ILogger logger)
		{
			base.ConfigureServices(services, hostingEnvironment, logger);

			var configuration = hostingEnvironment.SecretConfigurationProvider.Get<UlearnConfiguration>(hostingEnvironment.SecretConfigurationSource);

			services.AddDbContext<AntiPlagiarismDb>(
				options => options.UseSqlServer(configuration.Database)
			);

			services.Configure<AntiPlagiarismConfiguration>(options =>
				options.SetFrom(hostingEnvironment.SecretConfigurationProvider.Get<AntiPlagiarismConfiguration>(hostingEnvironment.SecretConfigurationSource)));

			services.AddSwaggerExamplesFromAssemblyOf<WebApplication>();
		}

		public override void ConfigureDi(IServiceCollection services, ILogger logger)
		{
			base.ConfigureDi(services, logger);

			/* Database repositories */
			/* TODO (andgein): make auto-discovering of repositories */
			services.AddScoped<IClientsRepo, ClientsRepo>();
			services.AddScoped<ISubmissionsRepo, SubmissionsRepo>();
			services.AddScoped<ISnippetsRepo, SnippetsRepo>();
			services.AddScoped<ITasksRepo, TasksRepo>();
			services.AddScoped<IWorkQueueRepo, WorkQueueRepo>();
			services.AddScoped<IMostSimilarSubmissionsRepo, MostSimilarSubmissionsRepo>();
			services.AddScoped<IManualSuspicionLevelsRepo, ManualSuspicionLevelsRepo>();

			/* Other services */
			services.AddScoped<PlagiarismDetector>();
			services.AddScoped<StatisticsParametersFinder>();
			services.AddSingleton<CodeUnitsExtractor>();
			services.AddScoped<SnippetsExtractor>();
			services.AddScoped<SubmissionSnippetsExtractor>();
			services.AddScoped<NewSubmissionHandler>();
			services.AddSingleton<AddNewSubmissionWorker>();
		}
	}
}