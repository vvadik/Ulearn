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
        public async Task PostToChannel(Comment comment)
        {
            try
            {
                var token = WebConfigurationManager.AppSettings["telegram-commentsbot-token"];
                if (string.IsNullOrWhiteSpace(token))
                    return;
                var api = new Telegram.Bot.TelegramBotClient(token);
                var url = "https://ulearn.me/Course/" + comment.CourseId + "/" + comment.SlideId;
                var text = "«" + comment.Text + "»\nîò " + comment.Author.VisibleName + "\n" + url; 
                await api.SendTextMessageAsync("@ulearncomments", text);
            }
            catch (Exception e)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
            }
        }
    }
}