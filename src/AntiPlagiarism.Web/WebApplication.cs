using System;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Controllers;
using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Hosting;
using Vostok.Instrumentation.AspNetCore;
using Vostok.Logging.Serilog;
using Vostok.Logging.Serilog.Enrichers;
using Vostok.Metrics;

namespace AntiPlagiarism.Web
{
    public class WebApplication : AspNetCoreVostokApplication
    {
        protected override void OnStarted(IVostokHostingEnvironment hostingEnvironment)
        {
            hostingEnvironment.MetricScope.SystemMetrics(1.Minutes());
        }

        protected override IWebHost BuildWebHost(IVostokHostingEnvironment hostingEnvironment)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.With<ThreadEnricher>()
                .Enrich.With<FlowContextEnricher>()
                .MinimumLevel.Debug()
                .WriteTo.Airlock(LogEventLevel.Information);
			
            if (hostingEnvironment.Log != null)
                loggerConfiguration = loggerConfiguration.WriteTo.VostokLog(hostingEnvironment.Log);
            var logger = loggerConfiguration.CreateLogger();
			
            return new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{hostingEnvironment.Configuration["port"]}/")
                .AddVostokServices()
                .ConfigureServices(s => ConfigureServices(s, hostingEnvironment, logger))
                .UseSerilog(logger)
                .Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                    app.UseVostok();
					if (env.IsDevelopment())
					{
						app.UseDeveloperExceptionPage();
						app.UseBrowserLink();
					}

					app.UseMvc();

					var database = app.ApplicationServices.GetService<AntiPlagiarismDb>();
					database.MigrateToLatestVersion();
				})
                .Build();
        }

		private void ConfigureServices(IServiceCollection services, IVostokHostingEnvironment hostingEnvironment, ILogger logger)
		{
			services.AddDbContext<AntiPlagiarismDb>(
				options => options.UseSqlServer(hostingEnvironment.Configuration["database"])
			);
			services.AddSingleton(logger);

			services.Configure<AntiPlagiarismConfiguration>(options => hostingEnvironment.Configuration.GetSection("antiplagiarism").Bind(options));
			
			/* Database repositories */
			/* TODO (andgein): make auto-discovering of repositories */
			services.AddScoped<IClientsRepo, ClientsRepo>();
			services.AddScoped<ISubmissionsRepo, SubmissionsRepo>();
			services.AddScoped<ISnippetsRepo, SnippetsRepo>();
			services.AddScoped<ITasksRepo, TasksRepo>();
			
			/* Other services */
			services.AddScoped<PlagiarismDetector>();
			services.AddScoped<StatisticsParametersFinder>();
			services.AddSingleton<CodeUnitsExtractor>();
			
			/* Asp.NET Core MVC */
			services.AddMvc();
		}
	}
}