using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.VisualBasic.FileIO;
using RunCsJob;
using RunCsJob.Api;
using uLearn.Extensions;
using uLearn.Model.Blocks;
using SearchOption = Microsoft.VisualBasic.FileIO.SearchOption;

namespace uLearn
{
	public class CourseValidator
	{
		private readonly List<Slide> slides;
		private readonly SandboxRunnerSettings settings;

		public event Action<string> InfoMessage;
		public event Action<string> Error;
		public event Action<string> Warning;

		public CourseValidator(List<Slide> slides, SandboxRunnerSettings settings)
		{
			this.slides = slides;
			this.settings = settings;
		}

		public void ValidateExercises()
		{
			foreach (var slide in slides.OfType<ExerciseSlide>())
			{
				LogSlideProcessing("Validate exercise", slide);

				if (slide.Exercise is ProjectExerciseBlock exercise)
				{
					if (exercise.SupressValidatorMessages || ExerciseFolderDoesntContainRequiredFiles(slide, exercise))
						continue;

					if (ExerciseDirectoryContainsSolutionFile(exercise))
						ReportWarningIfWrongAnswersAreSolutionsOrNotOk(slide, exercise);
					else
						ReportSlideWarning(slide, $"Exercise directory doesn't contain {exercise.CorrectSolutionFileName}");

					ReportErrorIfInitialCodeIsSolutionOrNotOk(slide, exercise);
					ReportIfStudentsZipHasErrors(slide, exercise);
				}
				else
					ReportErrorIfEthalonSolutionIsNotRight(slide);
			}
		}

		private bool ExerciseFolderDoesntContainRequiredFiles(ExerciseSlide slide, ProjectExerciseBlock ex)
		{
			var exerciseFilesRelativePaths = FileSystem.GetFiles(ex.ExerciseFolder.FullName, SearchOption.SearchAllSubDirectories)
				.Select(path => new FileInfo(path).GetRelativePath(ex.ExerciseFolder.FullName))
				.ToList();

			return ExerciseFolderDoesntContainCsproj() || ExerciseFolderDoesntContainUserCodeFile();

			bool ExerciseFolderDoesntContainUserCodeFile() => ReportErrorIfExerciseFolderDoesntContainFile(ex.UserCodeFileName);
			bool ExerciseFolderDoesntContainCsproj() => ReportErrorIfExerciseFolderDoesntContainFile(ex.CsprojFileName);

			bool ReportErrorIfExerciseFolderDoesntContainFile(string path)
			{
				if (exerciseFilesRelativePaths.Any(p => p.Equals(path, StringComparison.InvariantCultureIgnoreCase)))
					return false;
				ReportSlideError(slide, $"Exercise folder ({ex.ExerciseFolder.Name}) doesn't contain ({path})");
				return true;
			}
		}

		private bool ExerciseDirectoryContainsSolutionFile(ProjectExerciseBlock ex)
			=> ex.ExerciseFolder.GetFiles().Any(f => f.Name.Equals(ex.CorrectSolutionFileName));

		private void ReportWarningIfWrongAnswersAreSolutionsOrNotOk(ExerciseSlide slide, ProjectExerciseBlock ex)
		{
			var filesWithWrongAnswer = FileSystem.GetFiles(ex.ExerciseFolder.FullName, SearchOption.SearchAllSubDirectories)
				.Select(name => new FileInfo(name))
				.Where(f => IsWrongAnswer(ex, f.Name));

			foreach (var waFile in filesWithWrongAnswer)
			{
				var submission = new ProjRunnerSubmission
				{
					Id = slide.Id.ToString(),
					ZipFileData = GetZipBytesWithWrongAnswer(ex, waFile),
					ProjectFileName = ex.CsprojFileName,
					Input = "",
					NeedRun = true,
				};
				var result = SandboxRunner.Run(submission);

				ReportWarningIfWrongAnswerVerdictIsNotOk(slide, waFile.Name, result);
				ReportWarningIfWrongAnswerIsSolution(slide, waFile.Name, result);
			}
		}

		private byte[] GetZipBytesWithWrongAnswer(ProjectExerciseBlock ex, FileInfo waFile)
		{
			return ex.ExerciseFolder.ToZip(new[] { ex.UserCodeFileName },
				new[]
				{
					new FileContent
					{
						Path = ex.CsprojFileName,
						Data = ProjModifier.ModifyCsproj(ex.CsprojFile, p => PrepareCsprojForCheckingWrongAnswer(p, ex, waFile))
					}
				});
		}

		private void PrepareCsprojForCheckingWrongAnswer(Project proj, ProjectExerciseBlock ex, FileInfo wrongAnswer)
		{
			var excludeSolution = proj.Items.Select(i => i.UnevaluatedInclude).Single(ex.IsCorrectSolution);

			ProjModifier.SetFilenameItemTypeToCompile(proj, wrongAnswer.Name);
			ProjModifier.PrepareForChecking(proj, ex.StartupObject, new[] { excludeSolution });
		}

		private void ReportWarningIfWrongAnswerVerdictIsNotOk(Slide slide, string waFileName, RunningResults waResult)
		{
			if (VerdictIsNotOk(waResult))
				ReportSlideWarning(slide, $"Code verdict of file with wrong answer ({waFileName}) is not OK. RunResult = " + waResult);
		}

		private static bool VerdictIsNotOk(RunningResults result)
		{
			return !result.Verdict.IsOneOf(Verdict.Ok, Verdict.MemoryLimit, Verdict.TimeLimit);
		}

		private void ReportWarningIfWrongAnswerIsSolution(Slide slide, string waFileName, RunningResults waResult)
		{
			if (IsSolution(waResult))
				ReportSlideWarning(slide, $"Code of file with wrong answer ({waFileName}) is solution!");
		}

		private static bool IsSolution(RunningResults result)
		{
			return result.Verdict == Verdict.Ok && result.Output == "";
		}

		private void ReportSlideWarning(Slide slide, string warning)
		{
			ReportWarning(slide.Title + ". " + warning);
		}

		private void ReportWarning(string message)
		{
			Warning?.Invoke(message);
		}

		public void ReportIfStudentsZipHasErrors(Slide slide, ProjectExerciseBlock ex)
		{
			var tempDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_student_zip_unzipped", ex.ExerciseFolder.Name));
			try
			{
				Utils.UnpackZip(ex.StudentsZip.Content(), tempDir.FullName);
				var res = MsBuildRunner.BuildProject(settings.MsBuildSettings, tempDir.GetFile(ex.CsprojFileName).FullName, tempDir);

				ReportErrorIfStudentsZipNotBuilding(slide, ex, res);
				ReportErrorIfStudentsZipHasWrongAnswerOrSolutionFiles(slide, tempDir);
				ReportErrorIfCsprojHasUserCodeOfNotCompileType(slide, ex, tempDir);
				ReportErrorIfCsprojHasWrongAnswerOrSolutionItems(slide, ex, tempDir);
			}
			finally
			{
				tempDir.Delete(true);
			}
		}

		private void ReportErrorIfStudentsZipNotBuilding(Slide slide, ProjectExerciseBlock ex, MSbuildResult res)
		{
			if (!res.Success)
				ReportSlideError(slide, ex.CsprojFileName + " not building! " + res);
		}

		private void ReportErrorIfStudentsZipHasWrongAnswerOrSolutionFiles(Slide slide, DirectoryInfo unpackedZipDir)
		{
			var wrongAnswersOrSolution = GetOrderedFileNames(unpackedZipDir, ProjectExerciseBlock.IsAnyWrongAnswerOrAnySolution);

			if (wrongAnswersOrSolution.Any())
				ReportSlideError(slide, $"Student zip exercise directory has 'wrong answer' and/or solution files ({string.Join(", ", wrongAnswersOrSolution)})");
		}

		private string[] GetOrderedFileNames(DirectoryInfo dir, Func<string, bool> predicate)
		{
			var allNames = dir.GetFiles().Select(f => f.Name);
			return GetOrderedFileNames(allNames, predicate);
		}

		private string[] GetOrderedFileNames(IEnumerable<string> names, Func<string, bool> predicate)
			=>
				names.Where(predicate).OrderBy(n => n).ToArray();

		private void ReportErrorIfCsprojHasUserCodeOfNotCompileType(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			var csproj = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();

			var userCode = new Project(csproj.FullName, null, null, new ProjectCollection()).Items
				.Single(i => i.UnevaluatedInclude.Equals(ex.UserCodeFileName));

			if (!userCode.ItemType.Equals("Compile"))
				ReportSlideError(slide, $"Student's csproj has user code item ({userCode.UnevaluatedInclude}) of not compile type");
		}

		private void ReportErrorIfCsprojHasWrongAnswerOrSolutionItems(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			var csprojFile = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();
			var csProj = new Project(csprojFile.FullName, null, null, new ProjectCollection());
			var csProjItems = csProj.Items.Select(i => i.UnevaluatedInclude);

			var wrongAnswersOrSolution = GetOrderedFileNames(csProjItems, ProjectExerciseBlock.IsAnyWrongAnswerOrAnySolution);

			if (wrongAnswersOrSolution.Any())
				ReportSlideError(slide, $"Student's csproj has 'wrong answer' and/or solution items ({string.Join(", ", wrongAnswersOrSolution)})");
		}

		private void LogSlideProcessing(string prefix, Slide slide)
		{
			InfoMessage?.Invoke(prefix + " " + slide.Info.Unit.Title + " - " + slide.Title);
		}

		public void ValidateVideos()
		{
			var videos = GetVideos().ToLookup(d => d.Item2, d => d.Item1);
			foreach (var g in videos.Where(g => g.Count() > 1))
				ReportError("Duplicate videos on slides " + string.Join(", ", g));
			foreach (var g in videos)
			{
				var slide = g.First();
				LogSlideProcessing("Validate video", slide);
				var url = "https://www.youtube.com/oembed?format=json&url=http://www.youtube.com/watch?v=" + g.Key;
				try
				{
					new WebClient().DownloadData(url);
				}
				catch (Exception e)
				{
					ReportError("Slide " + slide + " contains not accessible video. " + e.Message);
				}
			}
		}

		private void ReportSlideError(Slide slide, string error)
		{
			ReportError(slide.Info.Unit.Title + ": " + slide.Title + ". " + error);
		}

		private void ReportError(string message)
		{
			Error?.Invoke(message);
		}

		public IEnumerable<Tuple<Slide, string>> GetVideos()
		{
			return slides
				.SelectMany(slide =>
					slide.Blocks.OfType<YoutubeBlock>()
						.Select(b => Tuple.Create(slide, b.VideoId)));
		}

		public void ReportErrorIfInitialCodeIsSolutionOrNotOk(ExerciseSlide slide, ProjectExerciseBlock ex)
		{
			var initialCode = ex.UserCodeFile.ContentAsUtf8();
			var submission = ex.CreateSubmission(slide.Id.ToString(), initialCode);
			var result = SandboxRunner.Run(submission);

			ReportErrorIfInitialCodeVerdictIsNotOk(slide, result);
			ReportErrorIfInitialCodeIsSolution(slide, result);
		}

		private void ReportErrorIfInitialCodeVerdictIsNotOk(ExerciseSlide slide, RunningResults result)
		{
			if (VerdictIsNotOk(result))
				ReportSlideError(slide, "Exercise initial code verdict is not OK. RunResult = " + result);
		}

		private void ReportErrorIfInitialCodeIsSolution(ExerciseSlide slide, RunningResults result)
		{
			if (IsSolution(result))
				ReportSlideError(slide, "Exercise initial code (available to students) is solution!");
		}

		private void ReportErrorIfEthalonSolutionIsNotRight(ExerciseSlide slide)
		{
			var exercise = (SingleFileExerciseBlock)slide.Exercise;
			var solution = exercise.BuildSolution(exercise.EthalonSolution);
			if (solution.HasErrors)
			{
				FailOnError(slide, solution, exercise.EthalonSolution);
				return;
			}
			if (solution.HasStyleIssues)
			{
				Console.WriteLine("Style issue: " + solution.StyleMessage);
			}

			var result = SandboxRunner.Run(exercise.CreateSubmission(
				slide.Id.ToString(),
				exercise.EthalonSolution), settings);

			var output = result.GetOutput().NormalizeEoln();

			var isRightAnswer = output.NormalizeEoln().Equals(slide.Exercise.ExpectedOutput.NormalizeEoln());
			if (!isRightAnswer)
			{
				ReportSlideError(slide,
					"ActualOutput: " + output.NormalizeEoln() + "\n" +
					"ExpectedOutput: " + slide.Exercise.ExpectedOutput.NormalizeEoln() + "\n" +
					"CompilationError: " + result.CompilationOutput + "\n" +
					"SourceCode: " + solution.SourceCode + "\n\n");
			}
		}

		private void FailOnError(ExerciseSlide slide, SolutionBuildResult solution, string ethalonSolution)
		{
			ReportSlideError(slide, $@"Template solution: {ethalonSolution}
source code: {solution.SourceCode}
error: {solution.ErrorMessage}");
		}

		private bool IsWrongAnswer(ProjectExerciseBlock ex, string name) =>
			Regex.IsMatch(name, ex.WrongAnswersAndSolutionNameRegexPattern) && !ex.IsCorrectSolution(name);
	}
}