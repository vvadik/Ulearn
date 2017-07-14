using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RunCsJob;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class CourseValidator : BaseValidator
	{
		private readonly List<Slide> slides;
		private readonly SandboxRunnerSettings settings;

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
					new ProjectExerciseValidator(this, settings, slide, exercise).ValidateExercises();
				}
				else
					ReportErrorIfEthalonSolutionIsNotRight(slide);
			}
		}

		private void LogSlideProcessing(string prefix, Slide slide)
		{
			LogInfoMessage(prefix + " " + slide.Info.Unit.Title + " - " + slide.Title);
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

		public IEnumerable<Tuple<Slide, string>> GetVideos()
		{
			return slides
				.SelectMany(slide =>
					slide.Blocks.OfType<YoutubeBlock>()
						.Select(b => Tuple.Create(slide, b.VideoId)));
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