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
			        ReportWarningIfExerciseDirDoesntContainSolutionFile(slide, exercise);
                    ReportWarningIfWrongAnswersAreSolutionsOrNotOk(slide, exercise);
			        ReportErrorIfInitialCodeIsSolutionOrNotOk(slide, exercise);
                    ReportIfStudentsZipHasErrors(slide, exercise);
			    }
				else
			        ReportErrorIfEthalonSolutionIsNotRight(slide);
            }
        }

	    public void ReportWarningIfExerciseDirDoesntContainSolutionFile(ExerciseSlide slide, ProjectExerciseBlock ex)
	    {
	        if (!ex.ExerciseFolder.GetFiles().Any(f => f.Name.Equals(ex.CorrectSolutionFileName)))
	            ReportSlideWarning(slide, $"Exercise directory doesn't contain {ex.CorrectSolutionFileName}");
	    }

	    public void ReportWarningIfWrongAnswersAreSolutionsOrNotOk(ExerciseSlide slide, ProjectExerciseBlock ex)
	    {
		    var filesWithWrongAnswer = FileSystem.GetFiles(ex.ExerciseFolder.FullName, SearchOption.SearchAllSubDirectories)
			    .Where(IsWrongAnswer)
			    .Select(name => new FileInfo(name));

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

		    bool IsWrongAnswer(string path) => Regex.IsMatch(path, ex.WrongAnswersAndSolutionNameRegexPattern);
		}

		private byte[] GetZipBytesWithWrongAnswer(ProjectExerciseBlock ex, FileInfo waFile)
		{
			return ex.ExerciseFolder.ToZip(new [] {ex.UserCodeFileName},
				new[]
				{
					new FileContent
					{
						Path = ex.CsprojFileName,
						Data = ProjModifier.ModifyCsproj(ex.CsprojFile, p => PrepareCsprojForCheckingWrongAnswer(p, ex, waFile))
					}
				});
		}

		private void PrepareCsprojForCheckingWrongAnswer(Project proj, ProjectExerciseBlock ex, FileInfo waFile)
		{
			var toExclude = proj.Items
				.Where(i => IsWrongAnswer(i) && NotCurrentWrongAnswer(i) || IsSolution(i))
				.Select(i => i.UnevaluatedInclude)
				.ToList();

			ProjModifier.SetFilenameItemTypeToCompile(proj, waFile.Name);
			ProjModifier.PrepareForChecking(proj, ex.StartupObject, toExclude);
			
			bool IsWrongAnswer(ProjectItem i) => Regex.IsMatch(i.UnevaluatedInclude, ex.WrongAnswersAndSolutionNameRegexPattern);
			bool NotCurrentWrongAnswer(ProjectItem i) => !i.UnevaluatedInclude.EndsWith(waFile.Name);
			bool IsSolution(ProjectItem i) => i.UnevaluatedInclude.Equals(ex.CorrectSolutionFileName);
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
			var tempDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "student_zip_unzipped"));
			try
			{
				Utils.UnpackZip(ex.StudentsZip.Content(), tempDir.FullName);
				var res = MsBuildRunner.BuildProject(settings.MsBuildSettings, tempDir.GetFile(ex.CsprojFileName).FullName, tempDir);

				ReportErrorIfStudentsZipNotBuilding(slide, ex, res);
				ReportErrorIfStudentZipHasSolution(slide, ex, tempDir);
				ReportErrorIfStudentZipHasWrongAnswerTests(slide, ex, tempDir);
				ReportErrorIfCsprojHasUserCodeOfNotCompileType(slide, ex, tempDir);
				ReportErrorIfCsprojHasSolutionItem(slide, ex, tempDir);
				ReportErrorIfCsprojHasWrongAnswerItems(slide, ex, tempDir);
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

		private void ReportErrorIfStudentZipHasSolution(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			if (unpackedZipDir.GetFiles().Any(f => f.Name.Equals(ex.CorrectSolutionFileName)))
				ReportSlideError(slide, $"Student zip exercise directory contains solution ({ex.CorrectSolutionFileName})");
		}

		private void ReportErrorIfStudentZipHasWrongAnswerTests(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			var wrongAnswers = unpackedZipDir.GetAllFiles()
				.Where(f => Regex.IsMatch(f.Name, ex.WrongAnswersAndSolutionNameRegexPattern))
				.Select(f => f.Name);
			var waNames = string.Join(", ", wrongAnswers);

			if (waNames.Any())
				ReportSlideError(slide, $"Student zip exercise directory contains wrong answer tests ({waNames})");
		}

		private void ReportErrorIfCsprojHasUserCodeOfNotCompileType(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			var csproj = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();
			var userCode = new Project(csproj.FullName, null, null, new ProjectCollection()).Items
				.Single(i => i.UnevaluatedInclude.Equals(ex.UserCodeFileName));

			if (!userCode.ItemType.Equals("Compile"))
				ReportSlideError(slide, $"Student zip csproj has user code item ({userCode.UnevaluatedInclude}) of not compile type");
		}

		private void ReportErrorIfCsprojHasSolutionItem(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			var csproj = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();
			var solution = new Project(csproj.FullName, null, null, new ProjectCollection()).Items
				.SingleOrDefault(i => i.UnevaluatedInclude.Equals(ex.CorrectSolutionFileName));

			if (solution != null)
				ReportSlideError(slide, $"Student zip csproj has solution item ({solution.UnevaluatedInclude})");
		}

		private void ReportErrorIfCsprojHasWrongAnswerItems(Slide slide, ProjectExerciseBlock ex, DirectoryInfo unpackedZipDir)
		{
			var csproj = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();
			var wrongAnswerItems = new Project(csproj.FullName, null, null, new ProjectCollection()).Items
				.Where(i => Regex.IsMatch(i.UnevaluatedInclude, ex.WrongAnswersAndSolutionNameRegexPattern))
				.Select(i => i.UnevaluatedInclude);
			var waItemNames = string.Join(", ", wrongAnswerItems);

			if (waItemNames.Any())
				ReportSlideError(slide, $"Student zip csproj has wrong answer items ({waItemNames})");
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
			var result =  SandboxRunner.Run(submission);

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
	}
}