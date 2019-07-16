using System;
using RunCsJob.Api;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Flashcards;

namespace uLearn.CourseTool.Validating
{
	public abstract class BaseValidator
	{
		protected BaseValidator()
		{
		}

		protected BaseValidator(BaseValidator fromValidator)
		{
			InfoMessage = fromValidator.InfoMessage;
			Error = fromValidator.Error;
			Warning = fromValidator.Warning;
		}

		public event Action<string> InfoMessage;
		public event Action<string> Error;
		public event Action<string> Warning;

		private static string FormatSlideIssueMessage(Slide slide, string warning)
		{
			return $"{slide.Info.Unit.Title}: {slide.Info.SlideFile?.Name} ({slide.Title})\n{warning}";
		}
		private static string FormatFlashcardIssueMessage(Flashcard flashcard, Slide slide, string warning)
		{
			return $" flashcard id {flashcard.Id} in slide {slide.Info.Unit.Title}: {slide.Info.SlideFile?.Name} ({slide.Title})\n{warning}";
		}

		protected void ReportSlideWarning(Slide slide, string warning)
		{
			ReportWarning(FormatSlideIssueMessage(slide, warning));
		}

		protected void ReportFlashcardWarning(Flashcard flashcard, Slide slide, string warning)
		{
			ReportWarning(FormatFlashcardIssueMessage(flashcard,slide, warning));
		}
		protected void ReportFlashcardError(Flashcard flashcard, Slide slide, string warning)
		{
			ReportError(FormatFlashcardIssueMessage(flashcard,slide, warning));
		}

		protected void ReportSlideError(Slide slide, string error)
		{
			ReportError(FormatSlideIssueMessage(slide, error));
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

		protected static bool IsCompiledAndExecuted(RunningResults result)
		{
			return result.Verdict.IsOneOf(Verdict.Ok, Verdict.OutputLimit, Verdict.MemoryLimit, Verdict.TimeLimit);
		}
	}
}