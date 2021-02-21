using System;
using System.Linq;
using AntiPlagiarism.Web.Database;

namespace ManualUtils.AntiPlagiarism
{
	public class FillLanguageToAntiplagiarism
	{
		private static void FillLanguageTasksStatisticsParameters(AntiPlagiarismDb adb)
		{
			var parameterses = adb
				.TasksStatisticsParameters
				.Where(p => p.Language == 0)
				.ToList();

			adb.DisableAutoDetectChanges();
			foreach (var parameterse in parameterses)
			{
				var submission = adb.Submissions.First(s => s.TaskId == parameterse.TaskId);
				try
				{
					parameterse.Language = submission.Language;
					adb.TasksStatisticsParameters.Update(parameterse);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on id {parameterse.TaskId} \"{submission.AdditionalInfo}\"");
				}
			}
			adb.SaveChanges();
			adb.EnableAutoDetectChanges();
		}

		private static void FillLanguageSnippetsStatistics(AntiPlagiarismDb adb)
		{
			var snippets = adb
				.SnippetsStatistics
				.Where(p => p.Language == 0)
				.ToList();

			adb.DisableAutoDetectChanges();
			foreach (var snippet in snippets)
			{
				var submission = adb.Submissions.First(s => s.TaskId == snippet.TaskId);
				try
				{
					snippet.Language = submission.Language;
					adb.SnippetsStatistics.Update(snippet);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on id {snippet.TaskId} \"{submission.AdditionalInfo}\"");
				}
			}
			adb.SaveChanges();
			adb.EnableAutoDetectChanges();
		}

		private static void FillLanguageManualSuspicionLevels(AntiPlagiarismDb adb)
		{
			var suspicionLevels = adb
				.ManualSuspicionLevels
				.Where(p => p.Language == 0)
				.ToList();

			adb.DisableAutoDetectChanges();
			foreach (var level in suspicionLevels)
			{
				var submission = adb.Submissions.First(s => s.TaskId == level.TaskId);
				try
				{
					level.Language = submission.Language;
					adb.ManualSuspicionLevels.Update(level);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on id {level.TaskId} \"{submission.AdditionalInfo}\"");
				}
			}
			adb.SaveChanges();
			adb.EnableAutoDetectChanges();
		}

		public static void FillLanguage(AntiPlagiarismDb adb)
		{
			FillLanguageSnippetsStatistics(adb);
			FillLanguageManualSuspicionLevels(adb);
			FillLanguageTasksStatisticsParameters(adb);
		}
	}
}