using System.Threading.Tasks;

namespace Notifications
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string to, string subject, string textBody, string htmlBody=null);
	}
}