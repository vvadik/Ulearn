using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Elmah;
using log4net;
using Telegram.Bot.Types.Enums;
using uLearn.Web.Models;

namespace uLearn.Web.Telegram
{
	public class CommentsBot : TelegramBot
	{
		private readonly ILog log = LogManager.GetLogger(typeof(CommentsBot));
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		public CommentsBot()
		{
			channel = WebConfigurationManager.AppSettings["ulearn.telegram.comments.channel"];
		}

		public async Task PostToChannel(Comment comment)
		{
			if (!IsBotEnabled)
				return;
			try
			{
				var text = CreatePostText(comment);
				await telegramClient.SendTextMessageAsync(channel, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
			}
			catch (Exception e)
			{
				log.Error($"Не могу отправить сообщение в телеграм-канал {channel}", e);
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
			}
		}

		private string CreatePostText(Comment comment)
		{
			var course = courseManager.GetCourse(comment.CourseId);
			var slide = course.FindSlideById(comment.SlideId);
			if (slide == null)
				return "";

			var slideTitle = $"{MakeNestedQuotes(course.Title)}: {MakeNestedQuotes(slide.Title)}";

			var url = "https://ulearn.me/Course/" + comment.CourseId + "/" + slide.Url + "#comment-" + comment.Id;
			var text = $"*{EscapeMarkdown(comment.Author.VisibleName)} в «{EscapeMarkdown(slideTitle)}»*\n{EscapeMarkdown(comment.Text.Trim())}\n\n{EscapeMarkdown(url)}";
			return text;
		}
	}
}