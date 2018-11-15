using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Units;

namespace Ulearn.Core.Courses.Slides.Quizzes
{
	public class QuizSlideLoader : ISlideLoader
	{
		public string Extension => ".quiz.xml";

		private static readonly Regex QuestionIdRegex = new Regex("^[0-9a-z_]+$", RegexOptions.IgnoreCase);

		public Slide Load(FileInfo file, Unit unit, int slideIndex, string courseId, CourseSettings settings)
		{
			var quiz = file.DeserializeXml<Quiz>();
			quiz.Meta?.FixPaths(file);

			var scoringGroupsIds = settings.Scoring.Groups.Keys;
			if (!string.IsNullOrEmpty(quiz.ScoringGroup) && !scoringGroupsIds.Contains(quiz.ScoringGroup))
				throw new CourseLoadingException(
					$"Неизвестная группа оценки у теста «{quiz.Title}»: {quiz.ScoringGroup}\n" +
					"Возможные значения: " + string.Join(", ", scoringGroupsIds));
			
			var questionBlocks = quiz.Blocks.OfType<AbstractQuestionBlock>().ToList();
			var questionIds = questionBlocks.Select(b => b.Id).ToImmutableHashSet();
			var questionIdsCount = questionIds.ToDictionary(id => id, id => questionBlocks.Count(b => b.Id == id));
			if (questionIdsCount.Values.Any(count => count > 1))
			{
				var repeatedQuestionId = questionIdsCount.First(kvp => kvp.Value > 1).Key;
				throw new CourseLoadingException(
					$"Идентификатор «{repeatedQuestionId}» в тесте «{quiz.Title}» принадлежит как минимум двум различным вопросам. " +
					"Идентификаторы вопросов (параметры id) должны быть уникальны.");
			}


			if (!questionIds.All(id => QuestionIdRegex.IsMatch(id)))
			{
				var badQuestionId = questionIds.First(id => !QuestionIdRegex.IsMatch(id));
				throw new CourseLoadingException(
					$"Идентификатор «{badQuestionId}» в тесте «{quiz.Title}» не удовлетворяет формату. " +
					"Идентификаторы вопросов (параметры id) должны состоять из латинских букв, цифр и символа подчёркивания. Идентификатор не может быть пустым.");
			}

			if (string.IsNullOrEmpty(quiz.ScoringGroup))
				quiz.ScoringGroup = settings.Scoring.DefaultScoringGroupForQuiz;

			BuildUp(quiz, unit, courseId, settings);
			quiz.InitQuestionIndices();
			var slideInfo = new SlideInfo(unit, file, slideIndex);
			return new QuizSlide(slideInfo, quiz);
		}

		public static void BuildUp(Quiz quiz, Unit unit, string courseId, CourseSettings settings)
		{
			var context = new BuildUpContext(unit, settings, null, courseId, quiz.Title);
			var blocks = quiz.Blocks.SelectMany(b => b.BuildUp(context, ImmutableHashSet<string>.Empty));
			quiz.Blocks = blocks.ToArray();
		}
	}
}