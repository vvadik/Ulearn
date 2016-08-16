using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using RunCsJob;
using RunCsJob.Api;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class CourseValidator
	{
		private readonly Course course;
		private readonly string workDir;

		public CourseValidator(Course course, string workDir)
		{
			this.course = course;
			this.workDir = workDir;
		}

		public void VideosAreOk()
		{
			var videos = GetVideos().ToLookup(d => d.Item2, d => d.Item1.Info.SlideFile.Name);
			foreach (var g in videos.Where(g => g.Count() > 1))
				Error("Duplicate videos on slides " + string.Join(", ", g));
			foreach (var g in videos)
			{
				var url = "https://www.youtube.com/oembed?format=json&url=http://www.youtube.com/watch?v=" + g.Key;
				new WebClient().DownloadData(url);
			}
		}

		private void Error(string message)
		{
			throw new Exception(message);
		}

		public IEnumerable<Tuple<Slide, string>> GetVideos()
		{
			return course.Slides
				.SelectMany(slide =>
					slide.Blocks.OfType<YoutubeBlock>()
						.Select(b => Tuple.Create(slide, b.VideoId)));
		}

		private void InitialCodeIsNotSolutionForProjExercise(ExerciseSlide slide)
		{
			var exercise = (ProjectExerciseBlock)slide.Exercise;
			var directoryName = Path.Combine(exercise.SlideFolderPath, exercise.ExerciseDir);
			var excluded = (exercise.PathsToExcludeForChecker ?? new string[0]).Concat(new[] { "bin/*", "obj/*" }).ToList();
			var exerciseDir = new DirectoryInfo(directoryName);
			var bytes = exerciseDir.ToZip(excluded, new[]
			{
				new FileContent
				{
					Path = exercise.CsprojFileName,
					Data = ProjModifier.ModifyCsproj(exerciseDir.GetFile(exercise.CsprojFileName),
						ProjModifier.PrepareCsprojBeforeZipping)
				}
			});
			var pathToCompiler = Path.Combine(workDir, "Microsoft.Net.Compilers.1.3.2");
			Console.WriteLine("pathToCompiler = " + pathToCompiler);
			var result = SandboxRunner.Run(pathToCompiler,
				new ProjRunnerSubmition
				{
					Id = slide.Id.ToString(),
					ZipFileData = bytes,
					ProjectFileName = exercise.CsprojFileName,
					Input = "",
					NeedRun = true
				});

			Console.WriteLine("Result = " + result);
			Assert.AreEqual(Verdict.Ok, result.Verdict);

			Assert.AreNotEqual(1.0, result.Score);
		}

		private void EthalonSolutionForSingleFileExercises(ExerciseSlide slide)
		{
			var exercise = (SingleFileExerciseBlock)slide.Exercise;
			var solution = exercise.BuildSolution(exercise.EthalonSolution);
			if (solution.HasErrors)
			{
				FailOnError(slide, solution, exercise.EthalonSolution);
				return;
			}

			var result = SandboxRunner.Run("", exercise.CreateSubmition(
				slide.Id.ToString(),
				exercise.EthalonSolution));

			var output = result.GetOutput().NormalizeEoln();

			var isRightAnswer = output.NormalizeEoln().Equals(slide.Exercise.ExpectedOutput.NormalizeEoln());
			if (!isRightAnswer)
			{
				Assert.Fail("mistake in: " + slide.Info.UnitName + " - " + slide.Title + "\n" +
							"\tActualOutput: " + output.NormalizeEoln() + "\n" +
							"\tExpectedOutput: " + slide.Exercise.ExpectedOutput.NormalizeEoln() + "\n" +
							"\tCompilationError: " + result.CompilationOutput + "\n" +
							"\tSourceCode: " + solution.SourceCode + "\n\n");
			}
		}

		public void AllExercisesAreOk()
		{
			foreach (var slide in course.Slides.OfType<ExerciseSlide>())
			{
				EthalonSolutionsForExercises(slide);
			}
		}
		public void EthalonSolutionsForExercises(ExerciseSlide slide)
		{
			if (slide.Exercise is ProjectExerciseBlock)
				InitialCodeIsNotSolutionForProjExercise(slide);
			else
				EthalonSolutionForSingleFileExercises(slide);
		}

		private static void FailOnError(ExerciseSlide slide, SolutionBuildResult solution, string ethalonSolution)
		{
			Assert.Fail($@"Template solution: {ethalonSolution}
  
  source code: {solution.SourceCode}
  
  solution has error in: {slide.Info.UnitName} - {slide.Title}
  
  error: {solution.ErrorMessage}");
		}
	}
}