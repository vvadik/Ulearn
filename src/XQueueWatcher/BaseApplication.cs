using System;
using System.Threading.Tasks;
using Database;
using Database.Di;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Ulearn.Core.Configuration;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Serilog;

namespace XQueueWatcher
{
	public abstract class BaseApplication : IVostokApplication
	{
		protected static ILogger logger;
		protected static UlearnConfiguration configuration;
		protected IServiceProvider serviceProvider;

		public virtual async Task InitializeAsync(IVostokHostingEnvironment hostingEnvironment)
		{
			var loggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.Sink(new VostokSink(hostingEnvironment.Log), LogEventLevel.Information);
			logger = loggerConfiguration.CreateLogger();

			serviceProvider = ConfigureServices(hostingEnvironment);
		}

		private IServiceProvider ConfigureServices(IVostokHostingEnvironment hostingEnvironment, Action<IServiceCollection> addServices = null)
		{
			var services = new ServiceCollection();
			services.AddLogging(builder => builder.AddSerilog(logger));

			configuration = hostingEnvironment.SecretConfigurationProvider.Get<UlearnConfiguration>(hostingEnvironment.SecretConfigurationSource);

			services.AddDbContextPool<UlearnDb>(
				options => options
					.UseLazyLoadingProxies()
					.UseSqlServer(configuration.Database)
			);

			services.Configure<UlearnConfiguration>(options =>
				options.SetFrom(hostingEnvironment.SecretConfigurationProvider.Get<UlearnConfiguration>(hostingEnvironment.SecretConfigurationSource)));

			ConfigureDi(services);

			addServices?.Invoke(services);

			return services.BuildServiceProvider();
		}

		private void ConfigureDi(IServiceCollection services)
		{
			services.AddSingleton(logger);

			services.AddDatabaseServices(logger);
		}

		public abstract Task RunAsync(IVostokHostingEnvironment environment);
	}
}