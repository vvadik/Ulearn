using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
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

        public CommentsBot()
        {
            token = WebConfigurationManager.AppSettings["telegram-commentsbot-token"];
            channel = WebConfigurationManager.AppSettings["telegram-commentsbot-channel"];
        }

        private string EscapeMarkdown(string text)
        {
            return Regex.Replace(text, @"([\[\]|\*_`])", @"\$1");
        }

        public async Task PostToChannel(Comment comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;
                var api = new Telegram.Bot.TelegramBotClient(token);
                var url = "https://ulearn.me/Course/" + comment.CourseId + "/" + comment.SlideId + "#comment-" + comment.Id;
                var text = $"*{EscapeMarkdown(comment.Author.VisibleName)}:*\n{EscapeMarkdown(comment.Text.Trim())}\n\n{url}";
                await api.SendTextMessageAsync(channel, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
            }
            catch (Exception e)
            {
                log.Error(e);
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
            }
        }
    }
}