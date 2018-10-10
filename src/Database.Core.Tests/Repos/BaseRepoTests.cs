using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace Database.Core.Tests.Repos
{
	public class BaseRepoTests
	{
		protected Logger logger;
		protected UlearnDb db;

		[SetUp]
		public virtual void SetUp()
		{
			logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.NUnitOutput()
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.ffff } {Level}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			var loggerFactory = new LoggerFactory(new List<ILoggerProvider> { new SerilogLoggerProvider(logger) });
			db = CreateDbContext(loggerFactory);
		}
		
		private static UlearnDb CreateDbContext(ILoggerFactory loggerFactory)
		{
			var optionsBuilder = new DbContextOptionsBuilder<UlearnDb>();
			optionsBuilder.UseLazyLoadingProxies();
			optionsBuilder.UseInMemoryDatabase("ulearn_test_database");
			if (loggerFactory != null)
				optionsBuilder.UseLoggerFactory(loggerFactory);
			
			return new UlearnDb(optionsBuilder.Options);
		}
	}
}