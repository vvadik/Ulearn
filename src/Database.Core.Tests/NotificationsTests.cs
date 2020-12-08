﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Extensions.Logging;

namespace Database.Core.Tests
{
	[TestFixture]
	public class NotificationsTests
	{
		private ServiceProvider serviceProvider;

		[SetUp]
		public void SetUp()
		{
			var loggerFactory = new LoggerFactory(new List<ILoggerProvider> { new SerilogLoggerProvider(Log.Logger) });
			var db = new UlearnDbFactory().CreateDbContext(new string[0], loggerFactory);

			/* Disable changetracker if needed */
			// db.ChangeTracker.AutoDetectChangesEnabled = false;

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton(db);
			serviceCollection.AddTransient<NotificationsRepo>();
			serviceCollection.AddTransient<FeedRepo>();
			serviceCollection.AddTransient<VisitsRepo>();

			serviceProvider = serviceCollection.BuildServiceProvider();
		}

		[TestCase("b189e18e-1866-47ec-8091-5a280a9c7945")]
		[Explicit]
		public async Task SlowNotificationsActualityChecking(string userId)
		{
			var feedRepo = serviceProvider.GetService<FeedRepo>();

			var transport = await feedRepo.GetUsersFeedNotificationTransportAsync(userId).ConfigureAwait(false);
			Console.WriteLine($@"Feed notification transport: {transport}");

			var notifications = await feedRepo.GetNotificationForFeedNotificationDeliveriesAsync<object>(userId, null, transport).ConfigureAwait(false);

			foreach (var notification in notifications)
			{
				Console.WriteLine($@"Checking actuality of notification {notification.GetNotificationType().ToString()} #{notification.Id}");
				var start = DateTime.Now;
				var isActual = notification.IsActual();
				var finish = DateTime.Now;
				Console.WriteLine($@"  Elapsed: {(finish - start).TotalMilliseconds} ms, isActual: {isActual}");
			}
		}
	}
}