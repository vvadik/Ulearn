using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using Elmah;
using log4net;
using Telegram.Bot.Types.Enums;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
    public class CommentsBot
    {
        private readonly string token;
        private readonly string channel;
        private readonly ILog log = LogManager.GetLogger(typeof(CommentsBot));
        private readonly CourseManager courseManager = WebCourseManager.Instance;

        public CommentsBot()
        {
            token = WebConfigurationManager.AppSettings["telegram-commentsbot-token"];
            channel = WebConfigurationManager.AppSettings["telegram-commentsbot-channel"];
        }

        private static string EscapeMarkdown(string text)
        {
            return Regex.Replace(text, @"([\[\]|\*_`])", @"\$1");
        }

        private static string MakeNestedQuotes(string text)
        {
            return Regex.Replace(text, "(\\s|^)[\"«]", @"$1„").Replace("[\"»]", @"“");
        }

        public async Task PostToChannel(Comment comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;

                var api = new Telegram.Bot.TelegramBotClient(token);
                
                var text = GetChannelPostText(comment);
                await api.SendTextMessageAsync(channel, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
            }
            catch (Exception e)
            {
                log.Error(e);
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
            }
        }

        private string GetChannelPostText(Comment comment)
        {
            var course = courseManager.GetCourse(comment.CourseId);
            var slide = course.FindSlideById(comment.SlideId);
            if (slide == null)
                return "";

            var slideTitle = $"{MakeNestedQuotes(course.Title)}: {MakeNestedQuotes(slide.Title)}";

            var url = "https://ulearn.me/Course/" + comment.CourseId + "/" + slide.Url + "#comment-" + comment.Id;
            var text = $"*{EscapeMarkdown(comment.Author.VisibleName)} â «{EscapeMarkdown(slideTitle)}»*\n{EscapeMarkdown(comment.Text.Trim())}\n\n{EscapeMarkdown(url)}";
            return text;
        }
    }
}