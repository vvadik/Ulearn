using System.Threading.Tasks;
using Database.Models;
using Telegram.Bot.Types.Enums;
using Ulearn.Common.Extensions;
using Ulearn.Core.RunCheckerJobApi;
using Ulearn.Core.Telegram;

namespace Ulearn.Web.Api.Controllers.Runner
{
	public class SandboxErrorsResultObserver : IResultObserver
	{
		private readonly ErrorsBot bot;

		public SandboxErrorsResultObserver(ErrorsBot errorsBot)
		{
			bot = errorsBot;
		}

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			/* Ignore all verdicts except SandboxError */
			if (result.Verdict != Verdict.SandboxError)
				return;

			var output = result.GetOutput();
			await bot.PostToChannelAsync(
				$"<b>Решение #{submission.Id} не запустилось в песочнице (SandboxError).</b>\n" +
				(string.IsNullOrEmpty(output) ? "" : $"Вывод:\n<pre>{output.EscapeHtml()}</pre>"),
				ParseMode.Html
			).ConfigureAwait(false);
		}
	}
}