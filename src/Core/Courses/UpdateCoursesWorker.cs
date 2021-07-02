using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Applications.Scheduled;
using Vostok.Hosting.Abstractions;

namespace Ulearn.Core.Courses
{
	public interface ICourseUpdater
	{
		Task UpdateCourses();
		Task UpdateTempCourses();
	}

	public class UpdateCoursesWorker : VostokScheduledApplication
	{
		private readonly ICourseUpdater courseUpdater;
		private readonly TimeSpan coursesUpdatePeriod = TimeSpan.FromMilliseconds(1000);
		private readonly TimeSpan tempCoursesUpdatePeriod = TimeSpan.FromMilliseconds(500);

		public UpdateCoursesWorker(ICourseUpdater courseUpdater)
		{
			this.courseUpdater = courseUpdater;
		}

		public override void Setup(IScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
		{
			RunUpdateCoursesWorker(builder);
		}

		private void RunUpdateCoursesWorker(IScheduledActionsBuilder builder)
		{
			builder.Schedule("UpdateCoursesWorker", Scheduler.Periodical(coursesUpdatePeriod), courseUpdater.UpdateCourses);
			builder.Schedule("UpdateCoursesWorker", Scheduler.Periodical(tempCoursesUpdatePeriod), courseUpdater.UpdateTempCourses);
		}

		private void RunUpdateCoursesWorker()
		{
			var coursesThread = new Thread(UpdateCoursesLoop);
			coursesThread.Start();
			var tempCoursesThread = new Thread(UpdateTempCoursesLoop);
			tempCoursesThread.Start();
		}

		private void UpdateCoursesLoop()
		{
			while (true)
			{
				courseUpdater.UpdateCourses().Wait();
				Thread.Sleep(coursesUpdatePeriod);
			}
		}

		private void UpdateTempCoursesLoop()
		{
			while (true)
			{
				courseUpdater.UpdateTempCourses().Wait();
				Thread.Sleep(tempCoursesUpdatePeriod);
			}
		}
	}
}