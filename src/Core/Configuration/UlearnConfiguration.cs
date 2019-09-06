using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace Ulearn.Core.Configuration
{
	public static class ApplicationConfiguration
	{
		private static readonly Lazy<IConfigurationRoot> configuration = new Lazy<IConfigurationRoot>(GetConfiguration);

		public static T Read<T>() where T : AbstractConfiguration
		{
			var r = configuration.Value.Get<T>();
			return r;
		}

		public static IConfigurationRoot GetConfiguration()
		{
			var applicationPath = string.IsNullOrEmpty(Utils.WebApplicationPhysicalPath)
				? AppDomain.CurrentDomain.BaseDirectory
				: Utils.WebApplicationPhysicalPath;
			var configurationBuilder = new ConfigurationBuilder()
				.SetBasePath(applicationPath);
			configurationBuilder.AddEnvironmentVariables();
			BuildAppsettingsConfiguration(configurationBuilder);
			return configurationBuilder.Build();
		}

		public static void BuildAppsettingsConfiguration(IConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			var environmentName = Environment.GetEnvironmentVariable("UlearnEnvironmentName");
			if (environmentName != null && environmentName.ToLower().Contains("local"))
				configurationBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
		}

		private static void DisposeConfiguration(IConfigurationRoot configuration) // https://github.com/aspnet/Extensions/issues/786
		{
			foreach (var provider in configuration.Providers.OfType<JsonConfigurationProvider>())
				if (provider.Source.FileProvider is PhysicalFileProvider pfp)
					pfp.Dispose();
		}
	}

	public abstract class AbstractConfiguration
	{
	}

	public class HostLogConfiguration
	{
		public bool Console { get; set; }

		public string PathFormat { get; set; }

		public string MinimumLevel { get; set; }

		public bool EnableEntityFrameworkLogging { get; set; }
	}

	public class UlearnConfiguration : AbstractConfiguration
	{
		public TelegramConfiguration Telegram { get; set; }

		public string BaseUrl { get; set; }

		public string CoursesDirectory { get; set; }

		public bool BuildExerciseStudentZips { get; set; }

		public string ExerciseStudentZipsDirectory { get; set; }

		public CertificateConfiguration Certificates { get; set; }

		public string GraphiteServiceName { get; set; }

		public string Database { get; set; }

		public GitConfiguration Git { get; set; }

		public string StatsdConnectionString { get; set; } // ConnectionString для подключения к Graphite-relay в формате "address=graphite-relay.com;port=8125;prefixKey=ulearn.local". Можно оставить пустой, чтобы не отправлять метрики

		public string SubmissionsUrl { get; set; } // Url to Ulearn.Web instance. I.E. https://ulearn.me

		public string RunnerToken { get; set; } // Must be equal on Ulearn.Web and RunC***Job instance

		public int? KeepAliveInterval { get; set; }
	}

	public class TelegramConfiguration
	{
		public string BotToken { get; set; }

		public ErrorsTelegramConfiguration Errors { get; set; }
	}

	public class ErrorsTelegramConfiguration
	{
		public string Channel { get; set; }
	}

	public class CertificateConfiguration
	{
		public string Directory { get; set; }
	}

	public class GitConfiguration
	{
		public GitWebhookConfiguration Webhook { get; set; }
	}

	public class GitWebhookConfiguration
	{
		public string Secret { get; set; }
	}
}