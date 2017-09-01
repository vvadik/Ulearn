using System;
using RunCsJob.Api;
using uLearn.Extensions;

namespace uLearn
{
	public abstract class BaseValidator
	{
		public event Action<string> InfoMessage;
		public event Action<string> Error;
		public event Action<string> Warning;

		protected BaseValidator()
		{
		}

		protected BaseValidator(BaseValidator fromValidator)
		{
			InfoMessage = fromValidator.InfoMessage;
			Error = fromValidator.Error;
			Warning = fromValidator.Warning;
		}

		protected void ReportSlideWarning(Slide slide, string warning)
		{
			ReportWarning(slide.Title + ". " + warning);
		}

		protected void ReportSlideError(Slide slide, string error)
		{
			ReportError(slide.Info.Unit.Title + ": " + slide.Title + ". " + error);
		}

		protected void ReportWarning(string message)
		{
			Warning?.Invoke(message);
		}

		protected void ReportError(string message)
		{
			Error?.Invoke(message);
		}

		protected void LogInfoMessage(string message)
		{
			InfoMessage?.Invoke(message);
		}

		protected static bool VerdictIsNotOk(RunningResults result)
		{
			return !result.Verdict.IsOneOf(Verdict.Ok, Verdict.MemoryLimit, Verdict.TimeLimit);
		}

		protected static bool IsSolution(RunningResults result)
		{
			return result.Verdict == Verdict.Ok && result.Output == "";
		}
	}
}