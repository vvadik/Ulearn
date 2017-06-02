using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notifications
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string to, string subject, string textContent = null, string htmlContent = null, EmailButton button = null, string textContentAfterButton = null, string htmlContentAfterButton = null);
	}

	public class EmailButton
	{
		public string Link;

		public string Text;
	}
}