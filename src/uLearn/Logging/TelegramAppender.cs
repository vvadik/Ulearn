using log4net.Appender;
using log4net.Core;
using Telegram.Bot.Types.Enums;
using uLearn.Telegram;
using Telegram.Bot.Exceptions;
using uLearn.Extensions;

namespace uLearn.Logging
{
	public class TelegramAppender : AppenderSkeleton
	{
		private static readonly ErrorsBot errorsBot = new ErrorsBot();
		
		protected override void Append(LoggingEvent loggingEvent)
		{
			/* Ignore telegram exceptions to stop infinity flood */
			if (loggingEvent.ExceptionObject is ApiRequestException)
				return;
			
			var message = $"*{loggingEvent.Level}* from `{loggingEvent.LoggerName.EscapeMarkdown()}`:\n{RenderLoggingEvent(loggingEvent).EscapeMarkdown()}";
			if (loggingEvent.ExceptionObject != null)
				message += $"```{loggingEvent.ExceptionObject.Message}\n\n{loggingEvent.ExceptionObject.StackTrace}```";
			
			errorsBot.PostToChannel(message, ParseMode.Markdown);
		}
		
		/// <summary>
		/// This appender requires a <see cref="log4net.Layout"/> to be set.
		/// </summary>
		protected override bool RequiresLayout => true;
	}
}