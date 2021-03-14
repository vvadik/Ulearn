using System;
using System.Collections.Generic;
using System.Linq;
using AntiPlagiarism.Web.Database;
using Ulearn.Common;

namespace ManualUtils.AntiPlagiarism
{

	public class FillLanguageToAntiplagiarism
	{
		private static Dictionary<Guid, Language> taskIdToSubmission = new Dictionary<Guid, Language>();

		private static Language GetLanguageByTaskId(Guid taskId, AntiPlagiarismDb adb)
		{
			if (!taskIdToSubmission.ContainsKey(taskId))
				taskIdToSubmission[taskId] = adb.Submissions.OrderByDescending(s => s.AddingTime).First(s => s.TaskId == taskId).Language;
			return taskIdToSubmission[taskId];
		}

		private static void FillLanguageTasksStatisticsParameters(AntiPlagiarismDb adb)
		{
			Console.WriteLine("FillLanguageTasksStatisticsParameters");

			var parameterses = adb
				.TasksStatisticsParameters
				.Where(p => p.Language == 0);

			var count = parameterses.Count();

			Console.WriteLine($"Count {parameterses.Count}");

			adb.DisableAutoDetectChanges();
			var completed = 0;
			foreach (var parameterse in parameterses)
			{
				completed++;
				try
				{
					parameterse.Language = GetLanguageByTaskId(parameterse.TaskId, adb);
					adb.TasksStatisticsParameters.Update(parameterse);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on id {parameterse.TaskId}: {ex}");
				}

				if (count % 1000 == 0)
				{
					Console.WriteLine($"FillLanguageTasksStatisticsParameters - Completed {completed} / {count}");
					adb.SaveChanges();
				}
			}

			adb.SaveChanges();
			adb.EnableAutoDetectChanges();
		}

		private static void FillLanguageSnippetsStatistics(AntiPlagiarismDb adb)
		{
			Console.WriteLine("FillLanguageSnippetsStatistics");

			var snippets = adb
				.SnippetsStatistics
				.Where(p => p.Language == 0);

			var count = snippets.Count();

			Console.WriteLine($"Count snippets {snippets.Count}");

			adb.DisableAutoDetectChanges();
			var completed = 0;
			foreach (var snippet in snippets)
			{
				completed++;
				try
				{
					snippet.Language = GetLanguageByTaskId(snippet.TaskId, adb);
					adb.SnippetsStatistics.Update(snippet);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on id {snippet.TaskId}: {ex}");
				}

				if (count % 1000 == 0)
				{
					Console.WriteLine($"FillLanguageSnippetsStatistics - Completed {completed} / {count}");
					adb.SaveChanges();
				}
			}

			adb.SaveChanges();
			adb.EnableAutoDetectChanges();
		}

		private static void FillLanguageManualSuspicionLevels(AntiPlagiarismDb adb)
		{
			Console.WriteLine("FillLanguageManualSuspicionLevels");

			var suspicionLevels = adb
				.ManualSuspicionLevels
				.Where(p => p.Language == 0);

			var count = suspicionLevels.Count();

			Console.WriteLine($"Count {suspicionLevels.Count}");

			adb.DisableAutoDetectChanges();
			var completed = 0;
			foreach (var level in suspicionLevels)
			{
				completed++;
				try
				{
					level.Language = GetLanguageByTaskId(level.TaskId, adb);;
					adb.ManualSuspicionLevels.Update(level);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error on id {level.TaskId}: {ex}");
				}

				if (count % 1000 == 0)
				{
					Console.WriteLine($"FillLanguageManualSuspicionLevels - Completed {completed} / {count}");
					adb.SaveChanges();
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