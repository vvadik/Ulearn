using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace Ulearn.Core.Configuration
{
	public static class ApplicationConfiguration
	{
		private static T Read<T>(IDictionary<string, string> initialData, bool isAppsettingsJsonOptional=false) where T : AbstractConfiguration
		{
			var configuration = GetConfiguration(initialData, isAppsettingsJsonOptional);
			var result = configuration.Get<T>();
			DisposeConfiguration(configuration); // https://github.com/aspnet/Extensions/issues/786
			return result;
		}

		private static void DisposeConfiguration(IConfigurationRoot configuration)
		{
			foreach (var provider in configuration.Providers.OfType<JsonConfigurationProvider>())
				if (provider.Source.FileProvider is PhysicalFileProvider pfp)
					pfp.Dispose();
		}
		
		public static T Read<T>(bool isAppsettingsJsonOptional=false) where T : AbstractConfiguration
		{
			return Read<T>(new Dictionary<string, string>(), isAppsettingsJsonOptional);
		}

		private static IConfigurationRoot GetConfiguration(IDictionary<string, string> initialData, bool isAppsettingsJsonOptional=false)
		{
			var applicationPath = string.IsNullOrEmpty(Utils.WebApplicationPhysicalPath)
				? AppDomain.CurrentDomain.BaseDirectory
				: Utils.WebApplicationPhysicalPath;
			var configurationBuilder = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.SetBasePath(applicationPath);
			configurationBuilder.AddEnvironmentVariables();
			BuildAppsettingsConfiguration(configurationBuilder);
			return configurationBuilder.Build();
		}

		public static void BuildAppsettingsConfiguration(IConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
			var environmentName = Environment.GetEnvironmentVariable("UlearnEnvironmentName");
			if(environmentName != null && environmentName.ToLower().Contains("local"))
				configurationBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);
		}
		
		public static IConfiguration GetConfiguration(bool isAppsettingsJsonOptional=false)
		{
			return GetConfiguration(new Dictionary<string, string>(), isAppsettingsJsonOptional);
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