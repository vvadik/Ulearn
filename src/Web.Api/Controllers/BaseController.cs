using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using uLearn;
using uLearn.Quizes;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Notifications;
using SlideInfo = Ulearn.Web.Api.Models.Common.SlideInfo;

namespace Ulearn.Web.Api.Controllers
{
	/* See https://docs.microsoft.com/en-us/aspnet/core/web-api/index?view=aspnetcore-2.1#annotate-class-with-apicontrollerattribute
	   for detailed description of ApiControllerAttribute */
	[ApiController]
	public class BaseController : Controller
	{
		protected readonly ILogger logger;
		protected readonly WebCourseManager courseManager;
		protected readonly UlearnDb db;

		public BaseController(ILogger logger, WebCourseManager courseManager, UlearnDb db)
		{
			this.logger = logger ?? throw new ArgumentException(nameof(logger));
			this.courseManager = courseManager ?? throw new ArgumentException(nameof(courseManager));
			this.db = db ?? throw new ArgumentException(nameof(db)); 
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			DisableEfChangesTrackingForGetRequests(context);
			
			await next().ConfigureAwait(false);
		}

		private void DisableEfChangesTrackingForGetRequests(ActionContext context)
		{
			/* Disable change tracking in EF Core for GET requests due to performance issues */
			/* TODO (andgein): we need a way to enable change tracking for some GET requests in future */
			var isRequestSafe = context.HttpContext.Request.Method == "GET"; // Maybe for HEAD and OPTION requests too?
			db.ChangeTracker.AutoDetectChangesEnabled = !isRequestSafe;

			if (isRequestSafe)
			{
				logger.Information("Выключаю автоматическое отслеживание изменений в EF Core: db.ChangeTracker.AutoDetectChangesEnabled = false");
			}
		}

		protected UnitInfo BuildUnitInfo(string courseId, Unit unit)
		{
			return new UnitInfo
			{
				Id = unit.Id,
				Title = unit.Title,
				Slides = unit.Slides.Select(slide => BuildSlideInfo(courseId, slide)).ToList()
			};
		}

		protected SlideInfo BuildSlideInfo(string courseId, Slide slide)
		{
			return new SlideInfo
			{
				Id = slide.Id,
				Title = slide.Title,
				Slug = slide.Url,
				ApiUrl = Url.Action(nameof(SlidesController.SlideInfo), "Slides", new { courseId = courseId, slideId = slide.Id }),
				MaxScore = slide.MaxScore,
				Type = GetSlideType(slide)
			};
		}

		protected static SlideType GetSlideType(Slide slide)
		{
			switch (slide)
			{
				case ExerciseSlide _:
					return SlideType.Exercise;
				case QuizSlide _:
					return SlideType.Quiz;
				default:
					return SlideType.Lesson;
			}
		}

		protected ShortUserInfo BuildShortUserInfo(ApplicationUser user, bool discloseLogin=false)
		{
			return new ShortUserInfo
			{
				Id = user.Id,
				Login = discloseLogin ? user.UserName : null,
				FirstName = user.FirstName ?? "",
				LastName = user.LastName ?? "",
				VisibleName = user.VisibleName,
				AvatarUrl = user.AvatarUrl,
				Gender = user.Gender,
			};
		}		

		protected CommentInfo BuildCommentInfo(Comment comment)
		{
			if (comment == null)
				return null;
			
			return new CommentInfo
			{
				Id = comment.Id,
				CourseId = comment.CourseId,
				SlideId = comment.SlideId,
				PublishTime = comment.PublishTime,
				Author = BuildShortUserInfo(comment.Author),
				Text = comment.Text,
			};
		}
	}
}