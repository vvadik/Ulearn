using System;
using System.Threading.Tasks;
using AntiPlagiarism.UpdateDb.Configuration;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using uLearn.Configuration;
using Vostok.Logging.Serilog.Enrichers;

namespace AntiPlagiarism.UpdateDb
{
	public class Program
	{
		public static void Main(string[] args)
		{
			new Program().RunAsync().Wait();
		}

		public async Task RunAsync()
		{
			Console.WriteLine(
				@"This tool will help you to update antiplagiarism database. " +
				@"I.e. in case when new logic for code units extraction have been added."
			);

			var provider = GetServiceProvider();
			var updater = provider.GetService<AntiPlagiarismSnippetsUpdater>();
			await updater.UpdateAsync().ConfigureAwait(false);
		}

		private ServiceProvider GetServiceProvider()
		{
			var configuration = ApplicationConfiguration.Read<AntiPlagiarismUpdateDbConfiguration>();
			
			var services = new ServiceCollection();

			services.AddOptions();
			
			services.Configure<AntiPlagiarismUpdateDbConfiguration>(ApplicationConfiguration.GetConfiguration());
			services.Configure<AntiPlagiarismConfiguration>(ApplicationConfiguration.GetConfiguration().GetSection("antiplagiarism"));
			
			services.AddSingleton(GetLogger(configuration));	
			services.AddScoped(_ => GetDatabase(configuration));
			services.AddScoped<AntiPlagiarismSnippetsUpdater>();
			services.AddScoped<ISnippetsRepo, SnippetsRepo>();
			services.AddScoped<ISubmissionsRepo, SubmissionsRepo>();
			services.AddSingleton<CodeUnitsExtractor>();
			services.AddSingleton<SnippetsExtractor>();
			services.AddSingleton<SubmissionSnippetsExtractor>();
			
			return services.BuildServiceProvider();
		}

		private AntiPlagiarismDb GetDatabase(AntiPlagiarismUpdateDbConfiguration configuration)
		{
			var optionsBuilder = new DbContextOptionsBuilder<AntiPlagiarismDb>();
			optionsBuilder.UseSqlServer(configuration.Database);
			
			return new AntiPlagiarismDb(optionsBuilder.Options);
		}

		private ILogger GetLogger(AntiPlagiarismUpdateDbConfiguration configuration)
		{
			var loggerConfiguration = new LoggerConfiguration()
				.Enrich.With<ThreadEnricher>()
				.Enrich.With<FlowContextEnricher>()
				.MinimumLevel.Debug();
			
			loggerConfiguration = loggerConfiguration
				.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u3} [{Thread}] {Message:l}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Information);

			var pathFormat = "logs/log-{Date}.log";
			var minimumLevel = LogEventLevel.Debug;
			loggerConfiguration = loggerConfiguration
				.WriteTo.RollingFile(
					pathFormat,
					outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u3} [{Thread}] {Message:l}{NewLine}{Exception}",
					restrictedToMinimumLevel: minimumLevel  
				);
			
			return loggerConfiguration.CreateLogger();
		}
	}
}