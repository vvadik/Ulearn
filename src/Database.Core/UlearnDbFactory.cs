using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Database
{
	public class UlearnDbFactory : IDesignTimeDbContextFactory<UlearnDb>
	{
		public UlearnDb CreateDbContext(string[] args)
		{
			return CreateDbContext(args, null);
		}
		
		public UlearnDb CreateDbContext(string[] args, ILoggerFactory loggerFactory)
		{
			var configuration = GetConfiguration();

			var optionsBuilder = new DbContextOptionsBuilder<UlearnDb>();
			optionsBuilder.UseLazyLoadingProxies();
			optionsBuilder.UseSqlServer(configuration.GetValue<string>("database"));
			if (loggerFactory != null)
				optionsBuilder.UseLoggerFactory(loggerFactory);
			
			return new UlearnDb(optionsBuilder.Options);
		}

		private static IConfigurationRoot GetConfiguration()
		{
			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false)
				.AddEnvironmentVariables();
			var environmentName = Environment.GetEnvironmentVariable("UlearnEnvironmentName");
			if(environmentName != null && environmentName.ToLower().Contains("local"))
				configurationBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
			return configurationBuilder.Build();
		}
	}
}