using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Core.Configuration;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Microsoft;

namespace Ulearn.Common.Api
{
	public abstract class BaseApplication : IVostokApplication
	{
		protected static UlearnConfiguration configuration;
		protected IServiceProvider serviceProvider;

		public virtual async Task InitializeAsync(IVostokHostingEnvironment hostingEnvironment)
		{
			var services = new ServiceCollection();
			ConfigureServices(services, hostingEnvironment);
			serviceProvider = services.BuildServiceProvider();
		}

		protected virtual void ConfigureServices(IServiceCollection services, IVostokHostingEnvironment hostingEnvironment)
		{
			services.AddLogging(builder => builder.AddVostok(hostingEnvironment.Log));
			configuration = hostingEnvironment.SecretConfigurationProvider.Get<UlearnConfiguration>(hostingEnvironment.SecretConfigurationSource);

			services.Configure<UlearnConfiguration>(options =>
				options.SetFrom(hostingEnvironment.SecretConfigurationProvider.Get<UlearnConfiguration>(hostingEnvironment.SecretConfigurationSource)));

			ConfigureDi(services);
		}

		protected virtual void ConfigureDi(IServiceCollection services)
		{
		}

		public abstract Task RunAsync(IVostokHostingEnvironment environment);
	}
}