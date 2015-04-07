using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using uLearn.Web.DataContexts;
using uLearn.Web.ExecutionService;
using uLearn.Web.Models;

namespace uLearn.Web
{
	public class ExecutionServiceMigration_Test
	{
		[Explicit]
		[Test]
		public void TestSources()
		{
			var logger = new Logger("logs");
			AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(@"..\App_Data\"));

			var parallelOptions = new ParallelOptions // if server, sandboxerRunner and test run in one machine
			{
				MaxDegreeOfParallelism = Environment.ProcessorCount
			};

			var res = Parallel.ForEach(GetSources(), parallelOptions, async source =>
			{
				var id = String.Format("{0:D10}", source.Id);
				var executionService = new CsSandboxExecutionService();
				SubmissionResult runResult = null;
				try
				{
					runResult = await executionService.Submit(source.Code, id);
				}
				catch (Exception ex)
				{
					logger.Write(source, ex);
				}
				if (runResult != null && !IsCorrectSolution(source, runResult))
					logger.Write(source, runResult);
			});

			while (!res.IsCompleted)
			{
				Thread.Sleep(60*1000);
			}
		}

		private static bool IsCorrectSolution(Solution solution, SubmissionResult runResult)
		{
			if (runResult == null)
				return false;

			if (runResult.IsInternalError)
				return false;

			var localValidationPrefixes = new[] { "Решение", "Строка", "Не нужно писать" };
			if (localValidationPrefixes.Any(s => solution.CompilationError.StartsWith(s)))
				return true;

			const string sphereEngineErrorMessage = "Ой-ой, Sphere Engine, проверяющий задачи, не работает. Попробуйте отправить решение позже.";
			if (solution.IsCompilationError && solution.CompilationError == sphereEngineErrorMessage)
				return true;

			const string oldCompilationError = "CompilationError";
			var isCompilationError = solution.IsCompilationError || solution.ActualOutput == oldCompilationError;
			var pseudoCompilationError = runResult.IsCompilationError || !string.IsNullOrEmpty(runResult.CompilationErrorMessage);

			if (isCompilationError != pseudoCompilationError && isCompilationError != runResult.IsCompilationError)
				return false;
			if (isCompilationError)
				return true;

			var output = runResult.GetOutput();
			var isRightAnswer = runResult.IsSuccess && solution.ExpectedOutput.Equals(output);
			return solution.IsRightAnswer == isRightAnswer;
		}

//		private static IEnumerable<Tuple<string, string, string>> GetErrors(string compilationError)
//		{
//			var lines = compilationError.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
//			var regex = new Regex(@"\((\d+),\d+\):\s+(warning|error)\s+CS(\d{4})");
//			foreach (var line in lines)
//			{
//				var m = regex.Match(line);
//				if (!m.Success)
//					continue;
//				var lineNumber = m.Groups[1].Value;
//				var type = m.Groups[2].Value;
//				var code = m.Groups[3].Value;
//				yield return Tuple.Create(lineNumber, type, code);
//			}
//		}
//
//		private static string RemoveFileName(string str)
//		{
//			var pos = str.IndexOf('(');
//			return pos == -1 ? "" : str.Substring(pos);
//		}
//
//		private static int SecondIndexOf(string str, char pattern)
//		{
//			var pos = str.IndexOf(pattern);
//			if (pos == -1)
//				return 0;
//			pos = str.IndexOf(pattern, pos + 1);
//			return pos == -1 ? 0 : pos;
//		}

		private static IEnumerable<Solution> GetSources()
		{
			var slides = WebCourseManager.Instance
				.GetCourses()
				.SelectMany(course => course.Slides)
				.Where(slide => slide is ExerciseSlide)
				.Cast<ExerciseSlide>()
				.ToDictionary(slide => slide.Id);

			var db = new ULearnDb();
			var solutions = db.UserSolutions.ToList();
			return solutions.Select(solution => new Solution(slides[solution.SlideId], solution));
		}
	}

	public class Solution
	{
		public Solution(ExerciseSlide slide, UserSolution solution)
		{
			Id = solution.Id;
			UserCode = GetText(solution.SolutionCode);
			Code = slide.Solution.BuildSolution(UserCode).SourceCode;
			IsCompilationError = solution.IsCompilationError;
			CompilationError = GetText(solution.CompilationError);
			ActualOutput = GetText(solution.Output);
			IsRightAnswer = solution.IsRightAnswer;
			ExpectedOutput = slide.ExpectedOutput.NormalizeEoln();
		}

		public readonly int Id;
		public readonly string UserCode;
		public readonly string Code;
		public readonly bool IsCompilationError;
		public readonly string CompilationError;
		public readonly string ActualOutput;
		public readonly string ExpectedOutput;
		public readonly bool IsRightAnswer;

		private static string GetText(TextBlob blob)
		{
			return blob == null ? null : blob.Text;
		}
	}

	public class Logger
	{
		private readonly string directory;
		private readonly JsonSerializerSettings jsonSettings;

		public Logger(string directory)
		{
			this.directory = directory;
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			foreach (var fileInfo in new DirectoryInfo(directory).GetFiles("*.json"))
			{
				fileInfo.Delete();
			}

			jsonSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented
			};
		}

		public void Write(Solution source, Object runResult)
		{
			var file = String.Format("{0:D10}.json", source.Id);
			var path = Path.Combine(directory, file);
			File.AppendAllText(path, JsonConvert.SerializeObject(new {source, runResult}, jsonSettings));
		}
	}
}