using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using log4net;
using Microsoft.AspNet.Identity;
using uLearn.Web.FilterAttributes;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class FeedController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(FeedController));

		private readonly CourseManager courseManager;

		private readonly NotificationsRepo notificationsRepo;
		private readonly FeedRepo feedRepo;

		public FeedController()
			: this(new ULearnDb(), WebCourseManager.Instance)
		{
		}

		protected override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);

			var userId = User.Identity.GetUserId();
			feedRepo.AddFeedNotificationTransportIfNeeded(userId).Wait();
		}

		public FeedController(ULearnDb db, CourseManager courseManager)
		{
			this.courseManager = courseManager;
			notificationsRepo = new NotificationsRepo(db);
			feedRepo = new FeedRepo(db);
		}
	}
}