using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class QuizController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();

		public QuizController()
			: this(CourseManager.AllCourses)
		{
		}

		public QuizController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		internal class QuizAnswer
		{
			public string QuizType;
			public string QuizId;
			public string ItemId;
			public string Text;

			public QuizAnswer(string type, string quizId, string itemId, string text)
			{
				QuizType = type;
				QuizId = quizId;
				ItemId = itemId;
				Text = text;
			}
		}


		internal class QuizInfoForDb
		{
			public string QuizId;
			public string ItemId;
			public string Text;
			public bool IsRightAnswer;
			public Type QuizType;
		}


		[HttpPost]
		[Authorize]
		public async Task<string> SubmitQuiz(string courseId, string slideIndex, string answer)
		{
			var intSlideIndex = int.Parse(slideIndex);
			var course = courseManager.GetCourse(courseId);
			if (userQuizzesRepo.IsQuizSlidePassed(courseId, User.Identity.GetUserId(), course.Slides[intSlideIndex].Id))
				return "already answered";
			var time = DateTime.Now;
			var quizzes = JsonConvert.DeserializeObject<List<QuizAnswer>>(answer).GroupBy(x => x.QuizId);
			var quizBlockWithTaskCount = ((QuizSlide) course.Slides[intSlideIndex]).Quiz.Blocks.Count(x => x is FillInBlock || x is IsTrueBlock || x is ChoiceBlock);
			var incorrectQuizzes = new List<string>();
			var fillInBlockType = typeof(FillInBlock);
			var tmpFolder = new List<QuizInfoForDb>();
			foreach (var ans in quizzes)
			{
				var quizInfo = GetQuizInfo(course, intSlideIndex, ans);
				foreach (var quizInfoForDb in quizInfo)
				{
					tmpFolder.Add(quizInfoForDb);
					if (!quizInfoForDb.IsRightAnswer && quizInfoForDb.QuizType == fillInBlockType)
						incorrectQuizzes.Add(ans.Key);
				}
			}
			var blocksInAnswerCount = tmpFolder.Select(x => x.QuizId).Distinct().Count();
			if (blocksInAnswerCount != quizBlockWithTaskCount)
				return "has empty bloks";
			foreach (var quizInfoForDb in tmpFolder)
				await userQuizzesRepo.AddUserQuiz(courseId, quizInfoForDb.IsRightAnswer, quizInfoForDb.ItemId, quizInfoForDb.QuizId,
					course.Slides[intSlideIndex].Id, quizInfoForDb.Text, User.Identity.GetUserId(), time);
			return string.Join("*", incorrectQuizzes.Distinct());
		}

		[HttpPost]
		public string GetRightAnswersToQuiz(string courseId, int slideIndex)
		{
			var course = courseManager.GetCourse(courseId);
			var quizSlide = course.Slides[slideIndex];
			var e = ((QuizSlide) quizSlide).RightAnswersToQuiz;
			return e;
		}

		private IEnumerable<QuizInfoForDb> GetQuizInfo(Course course, int slideIndex, IGrouping<string, QuizAnswer> answer)
		{
			var slide = course.Slides[slideIndex] as QuizSlide;
			if (slide == null)
				throw new Exception("Error in func 'GetSlideUsingId' or in 'QuizSlideId'");
			var block = slide.GetBlockById(answer.Key);
			if (block is FillInBlock)
				return CreateQuizInfoForDb(block as FillInBlock, answer.First().Text);
			if (block is ChoiceBlock)
				return CreateQuizInfoForDb(block as ChoiceBlock, answer);
			return CreateQuizInfoForDb(block as IsTrueBlock, answer);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(IsTrueBlock isTrueBlock, IGrouping<string, QuizAnswer> data)
		{
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = isTrueBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrueBlock.Answer.ToString() == data.First().ItemId,
					Text = data.First().ItemId,
					QuizType = typeof(IsTrueBlock)
				}
			};
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(ChoiceBlock choiseBlock, IGrouping<string, QuizAnswer> data)
		{
			if (!choiseBlock.Multiple)
				return new List<QuizInfoForDb>
				{
					new QuizInfoForDb
					{
						QuizId = choiseBlock.Id,
						ItemId = data.First().ItemId,
						IsRightAnswer = choiseBlock.Items.First(x => x.Id == data.First().ItemId).IsCorrect,
						Text = null,
						QuizType = typeof (ChoiceBlock)
					}
				};
			var ans = data.Select(x => x.ItemId).ToList()
				.Select(x => new QuizInfoForDb
				{
					QuizId = choiseBlock.Id,
					IsRightAnswer = false,
					ItemId = x,
					Text = null,
					QuizType = typeof(ChoiceBlock)

				}).ToList();
			var correctItems = new HashSet<string>(choiseBlock.Items.Where(x => x.IsCorrect).Select(x => x.Id));
			var ansItem = new HashSet<string>(ans.Select(x => x.ItemId));
			var count = ansItem.Count(correctItems.Contains);
			if (count != correctItems.Count || count != ansItem.Count) return ans;
			foreach (var info in ans)
				info.IsRightAnswer = true;
			return ans;
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(FillInBlock fillInBlock, string data)
		{
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = fillInBlock.Id,
					ItemId = null,
					IsRightAnswer = fillInBlock.Regexes.Any(regex => Regex.IsMatch(data, regex)),
					Text = data,
					QuizType = typeof(FillInBlock)
				}
			};
		}
	}
}