using System;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Applications.Scheduled;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;
using System.Threading.Tasks;
using Database.Repos;

namespace Ulearn.Web.Api.Workers
{
	public class RefreshGoogleSheetWorker : VostokScheduledApplication
	{
		
		private readonly IServiceScopeFactory serviceScopeFactory;
		private static ILog log => LogProvider.Get().ForContext(typeof(ArchiveGroupsWorker));
		
		public RefreshGoogleSheetWorker(IServiceScopeFactory serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory;
		}
		
		public override void Setup(IScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
		{
			var scheduler = Scheduler.Multi(Scheduler.Periodical(TimeSpan.FromMinutes(10)), Scheduler.OnDemand(out var refreshGoogleSheets));
			builder.Schedule("RefreshGoogleSheets", scheduler, RefreshGoogleSheets);
		}
		
		private async Task RefreshGoogleSheets()
		{
			log.Info("RefreshGoogleSheets");
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var googleSheetExportTasksRepo = scope.ServiceProvider.GetService<IGoogleSheetExportTasksRepo>();
				// var f = googleSheetExportTasksRepo.
			}
			log.Info("End RefreshGoogleSheets");
		}
	}
}