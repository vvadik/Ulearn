using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public class ProjectExerciseValidator : BaseValidator
	{
		private readonly ProjectExerciseBlock ex;
		private readonly ExerciseSlide slide;
		private readonly SandboxRunnerSettings settings;

		public ProjectExerciseValidator(BaseValidator baseValidator, SandboxRunnerSettings settings, ExerciseSlide slide, ProjectExerciseBlock exercise)
			: base(baseValidator)
		{
			this.settings = settings;
			this.slide = slide;
			ex = exercise;
		}

		public void ValidateExercises()
		{
			if (ReportErrorIfExerciseFolderMissesRequiredFiles())
				return;

			ReportWarningIfWrongAnswersAreSolutionsOrNotOk();

			if (!ExerciseDirectoryContainsSolutionFile())
				ReportSlideWarning(slide, $"Exercise directory doesn't contain {ex.CorrectSolutionFileName}");

			ReportErrorIfStudentsZipHasErrors();

			if (ex.DisableUserCodeFileValidations)
				return;

			ReportErrorIfInitialCodeIsSolutionOrNotOk();
		}

		private bool ReportErrorIfExerciseFolderMissesRequiredFiles()
		{
			var exerciseFilesRelativePaths = FileSystem.GetFiles(ex.ExerciseFolder.FullName, SearchOption.SearchAllSubDirectories)
				.Select(path => new FileInfo(path).GetRelativePath(ex.ExerciseFolder.FullName))
				.ToList();

			return ReportErrorIfMissingCsproj() || ReportErrorIfMissingUserCodeFile();

			bool ReportErrorIfMissingUserCodeFile() => ReportErrorIfMissingFile(ex.UserCodeFileName);
			bool ReportErrorIfMissingCsproj() => ReportErrorIfMissingFile(ex.CsprojFileName);

			bool ReportErrorIfMissingFile(string path)
			{
				if (exerciseFilesRelativePaths.Any(p => p.Equals(path, StringComparison.InvariantCultureIgnoreCase)))
					return false;
				ReportSlideError(slide, $"Exercise folder ({ex.ExerciseFolder.Name}) doesn't contain ({path})");
				return true;
			}
		}

		private bool ExerciseDirectoryContainsSolutionFile()
			=> ex.ExerciseFolder.GetFiles().Any(f => f.Name.Equals(ex.CorrectSolutionFileName));

		private void ReportWarningIfWrongAnswersAreSolutionsOrNotOk()
		{
			var filesWithWrongAnswer = FileSystem.GetFiles(ex.ExerciseFolder.FullName, SearchOption.SearchAllSubDirectories)
				.Select(name => new FileInfo(name))
				.Where(f => IsWrongAnswer(f.Name));

			foreach (var waFile in filesWithWrongAnswer)
			{
				var result = SandboxRunner.Run(ex.CreateSubmission(waFile.Name, waFile.ContentAsUtf8()));

				ReportWarningIfWrongAnswerVerdictIsNotOk(waFile.Name, result);
				ReportWarningIfWrongAnswerIsSolution(waFile.Name, result);
			}
		}

		private bool IsWrongAnswer(string name) =>
			Regex.IsMatch(name, ex.WrongAnswersAndSolutionNameRegexPattern) && !ex.IsCorrectSolution(name);

		private void ReportWarningIfWrongAnswerVerdictIsNotOk(string waFileName, RunningResults waResult)
		{
			if (VerdictIsNotOk(waResult))
				ReportSlideWarning(slide, $"Code verdict of file with wrong answer ({waFileName}) is not OK. RunResult = " + waResult);
		}

		private void ReportWarningIfWrongAnswerIsSolution(string waFileName, RunningResults waResult)
		{
			if (IsSolution(waResult))
				ReportSlideWarning(slide, $"Code of file with wrong answer ({waFileName}) is solution!");
		}

		private void ReportErrorIfInitialCodeIsSolutionOrNotOk()
		{
			var initialCode = ex.UserCodeFile.ContentAsUtf8();
			var submission = ex.CreateSubmission(slide.Id.ToString(), initialCode);
			var result = SandboxRunner.Run(submission);

			ReportErrorIfInitialCodeVerdictIsNotOk(result);
			ReportErrorIfInitialCodeIsSolution(result);
		}

		private void ReportErrorIfInitialCodeVerdictIsNotOk(RunningResults result)
		{
			if (VerdictIsNotOk(result))
				ReportSlideError(slide, "Exercise initial code verdict is not OK. RunResult = " + result);
		}

		private void ReportErrorIfInitialCodeIsSolution(RunningResults result)
		{
			if (IsSolution(result))
				ReportSlideError(slide, "Exercise initial code (available to students) is solution!");
		}

		public void ReportErrorIfStudentsZipHasErrors()
		{
			var tempExFolder = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExerciseFolder_From_StudentZip"));
			Utils.UnpackZip(ex.StudentsZip.Content(), tempExFolder.FullName);
			try
			{
				ReportErrorIfStudentsZipHasWrongAnswerOrSolutionFiles(tempExFolder);
				ReportErrorIfCsprojHasUserCodeOfNotCompileType(tempExFolder);
				ReportErrorIfCsprojHasWrongAnswerOrSolutionItems(tempExFolder);

				if (ex.DisableUserCodeFileValidations)
					return;

				var buildResult = MsBuildRunner.BuildProject(settings.MsBuildSettings, ex.CsprojFile.Name, tempExFolder);
				ReportErrorIfStudentsZipNotBuilding(buildResult);
			}
			finally
			{
				tempExFolder.Delete(true);
			}
		}

		private void ReportErrorIfStudentsZipNotBuilding(MSbuildResult res)
		{
			if (!res.Success)
				ReportSlideError(slide, ex.CsprojFileName + " not building! " + res);
		}

		private void ReportErrorIfStudentsZipHasWrongAnswerOrSolutionFiles(DirectoryInfo unpackedZipDir)
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

		private void ReportErrorIfCsprojHasUserCodeOfNotCompileType(DirectoryInfo unpackedZipDir)
		{
			var csproj = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();

			var userCode = new Project(csproj.FullName, null, null, new ProjectCollection()).Items
				.Single(i => i.UnevaluatedInclude.Equals(ex.UserCodeFileName));

			if (!userCode.ItemType.Equals("Compile"))
				ReportSlideError(slide, $"Student's csproj has user code item ({userCode.UnevaluatedInclude}) of not compile type");
		}

		private void ReportErrorIfCsprojHasWrongAnswerOrSolutionItems(DirectoryInfo unpackedZipDir)
		{
			var csprojFile = unpackedZipDir.GetFiles(ex.CsprojFileName).Single();
			var csProj = new Project(csprojFile.FullName, null, null, new ProjectCollection());
			var csProjItems = csProj.Items.Select(i => i.UnevaluatedInclude);

			var wrongAnswersOrSolution = GetOrderedFileNames(csProjItems, ProjectExerciseBlock.IsAnyWrongAnswerOrAnySolution);

			if (wrongAnswersOrSolution.Any())
				ReportSlideError(slide, $"Student's csproj has 'wrong answer' and/or solution items ({string.Join(", ", wrongAnswersOrSolution)})");
		}
	}
}