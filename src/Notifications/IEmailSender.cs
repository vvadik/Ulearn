using System.Threading.Tasks;
using Database.Models;
using Ulearn.Common.Extensions;

namespace Notifications
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string to, string subject, string textContent = null, string htmlContent = null, EmailButton button = null, string textContentAfterButton = null, string htmlContentAfterButton = null);
	}

	public class EmailButton
	{
		public EmailButton(NotificationButton notificationButton)
		{
			Link = notificationButton.Link;
			Link = Link.AddQueryParameter("utm_source", "email");

			Text = notificationButton.Text;
		}

		public string Link { get; private set; }
		public string Text { get; private set; }
	}
}