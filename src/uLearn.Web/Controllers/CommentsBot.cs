using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Elmah;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
    public class CommentsBot
    {
        private string token;
        private string channel;

        public CommentsBot()
        {
            token = WebConfigurationManager.AppSettings["telegram-commentsbot-token"];
            channel = WebConfigurationManager.AppSettings["telegram-commentsbot-channel"];
        }

        public async Task PostToChannel(Comment comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;
                var api = new Telegram.Bot.TelegramBotClient(token);
                var url = "https://ulearn.me/Course/" + comment.CourseId + "/" + comment.SlideId + "#comment-" + comment.Id;
                var text = "«" + comment.Text.Trim() + "»\nauthor: " + comment.Author.VisibleName + "\n" + url; 
                await api.SendTextMessageAsync(channel, text);
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
            }
        }
    }
}