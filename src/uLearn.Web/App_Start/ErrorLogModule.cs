using Elmah;
using log4net;
using uLearn.Web.Telegram;

namespace uLearn.Web
{
	public class ErrorLogModule : Elmah.ErrorLogModule
	{
		private readonly ErrorsBot errorsBot;
		private static readonly ILog log = LogManager.GetLogger(typeof(ErrorLogModule));

		public ErrorLogModule()
		{
			Logged += OnLogged;
			errorsBot = new ErrorsBot();
		}
		
		private void OnLogged(object sender, ErrorLoggedEventArgs args)
		{
			var error = args.Entry.Error;
			var entryId = args.Entry.Id;
			log.Error($"Произошла ошибка {entryId} (код {error.StatusCode}, подробности в Elmah):\n" +
					  $"Query string: {error.QueryString}",
				error.Exception);
			errorsBot.PostToChannel(entryId, error);
		}
	}
}