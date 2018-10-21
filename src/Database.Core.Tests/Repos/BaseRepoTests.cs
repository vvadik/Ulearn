using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Di;
using Database.Models;
using Database.Repos;
using LtiLibrary.NetCore.Lti.v2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Ulearn.Common;
using Z.EntityFramework.Plus;

namespace Database.Core.Tests.Repos
{
	public class BaseRepoTests
	{
		protected Logger logger;
		protected UlearnDb db;
		protected IServiceProvider serviceProvider;
		protected UlearnUserManager userManager;

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

			serviceProvider = ConfigureServices();
			
			userManager = serviceProvider.GetService<UlearnUserManager>();
			
			/* Configuring Z.EntityFramework.Plus for working with In-Memory database
			   See https://entityframework-plus.net/batch-delete for details */
			BatchDeleteManager.InMemoryDbContextFactory = () => CreateDbContext(loggerFactory);
		}

		private IServiceProvider ConfigureServices()
		{
			var services = new ServiceCollection();
			
			services.AddSingleton(db);
			services.AddLogging(builder => builder.AddSerilog(logger));
			services.AddDatabaseServices(logger);
			services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<UlearnDb>();

			return services.BuildServiceProvider();
		}

		protected static UlearnDb CreateDbContext(ILoggerFactory loggerFactory)
		{
			var optionsBuilder = new DbContextOptionsBuilder<UlearnDb>();
			optionsBuilder.UseLazyLoadingProxies();
			optionsBuilder.UseInMemoryDatabase("ulearn_test_database");
			if (loggerFactory != null)
				optionsBuilder.UseLoggerFactory(loggerFactory);
			
			return new UlearnDb(optionsBuilder.Options);
		}

		protected async Task<ApplicationUser> CreateUserAsync(string userName, string password=null)
		{
			if (password == null)
				password = StringUtils.GenerateSecureAlphanumericString(10);
			
			var user = new ApplicationUser { UserName = userName };
			var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);
			if (! result.Succeeded)
				throw new InvalidOperationException($"Can't create user {userName} with password {password}:\n{string.Join("\n", result.Errors.Select(e => e.Description))}");
			
			logger.Information($"User {userName} with password {password} successfully created");

			return await userManager.FindByNameAsync(userName).ConfigureAwait(false);
		}
	}
}