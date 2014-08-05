using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Ideone;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UserQuestionsRepo userQuestionsRepo = new UserQuestionsRepo();
		private readonly ExecutionService executionService = new ExecutionService();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();

		public CourseController()
			: this(CourseManager.AllCourses)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public async Task<ActionResult> Slide(string courseId, int slideIndex = 0)
		{
			var model = await CreateCoursePageModel(courseId, slideIndex);
			return View(model);
		}

		private async Task<CoursePageModel> CreateCoursePageModel(string courseId, int slideIndex)
		{
			Course course = courseManager.GetCourse(courseId);
			var isPassedTask = solutionsRepo.IsUserPassedTask(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId());
			await VisitSlide(courseId, course.Slides[slideIndex].Id);
			var model = new CoursePageModel
			{
				Course = course,
				SlideIndex = slideIndex,
				Slide = course.Slides[slideIndex],
				NextSlideIndex = slideIndex + 1,
				PrevSlideIndex = slideIndex - 1,
				IsPassedTask = isPassedTask,
				LatestAcceptedSolution = isPassedTask ? solutionsRepo.GetLatestAcceptedSolution(courseId, course.Slides[slideIndex].Id, User.Identity.GetUserId()) : null,
				Rate = GetRate(course.Id, course.Slides[slideIndex].Id),
				SolvedSlide = solutionsRepo.GetIndexesOfPassedSlide(course.Id, User.Identity.GetUserId()),
				VisitedSlide = visitersRepo.GetIndexesOfVisitedSlide(course.Id, User.Identity.GetUserId())
			};
			return model;
		}

		[Authorize]
		public async Task<ActionResult> AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var coursePageModel = await CreateCoursePageModel(courseId, slideIndex);
			var solutions = coursePageModel.IsPassedTask
				? solutionsRepo.GetAllAcceptedSolutions(courseId, coursePageModel.Course.Slides[slideIndex].Id)
				: new List<AcceptedSolutionInfo>();
			foreach (var solution in solutions)
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);

			var model = new AcceptedSolutionsPageModel
			{
				CoursePageModel = coursePageModel,
				AcceptedSolutions = solutions
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> RunSolution(string courseId, int slideIndex = 0)
		{
			var code = GetUserCode(Request.InputStream);
			var exerciseSlide = courseManager.GetExerciseSlide(courseId, slideIndex);
			var result = await CheckSolution(exerciseSlide, code, slideIndex);
			await SaveUserSolution(courseId, exerciseSlide.Id, code, result.CompilationError, result.ActualOutput, result.IsRightAnswer);
			return Json(result);
		}

		[HttpPost]
		[Authorize]
		public ContentResult PrepareSolution(string courseId, int slideIndex = 0)
		{
			var code = GetUserCode(Request.InputStream);
			var exerciseSlide = courseManager.GetExerciseSlide(courseId, slideIndex);
			var solution = exerciseSlide.Solution.BuildSolution(code);
			return Content(solution, "text/plain");
		}

		[HttpPost]
		[Authorize]
		public ActionResult FakeRunSolution(string courseId, int slideIndex = 0)
		{
			return Json(new RunSolutionResult
			{
				IsRightAnswer = false,
				ActualOutput = string.Join("\n", Enumerable.Repeat("Тридцать три корабля лавировали лавировали, да не вылавировали", 21)),
				ExpectedOutput = string.Join("\n", Enumerable.Repeat("Тридцать три корабля лавировали лавировали, да не вылавировали", 15)),
			});
		}

		[HttpPost]
		[Authorize]
		public async Task<string> AddQuestion(string title, string unitName, string question)
		{
			var userName = User.Identity.GetUserName();
			await userQuestionsRepo.AddUserQuestion(question, title, userName, unitName, DateTime.Now);
			return "Success!";
		}

		[HttpPost]
		[Authorize]
		public async Task<string> ApplyRate(string courseId, string slideId, string rate)
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates)Enum.Parse(typeof(SlideRates), rate);
			await slideRateRepo.AddRate(courseId, slideId, userId, slideRate);
			return "success!";
		}

		[HttpPost]
		[Authorize]
		public string GetRate(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			return slideRateRepo.FindRate(courseId, slideId, userId);
		}

		[HttpPost]
		[Authorize]
		public async Task<string> AddHint(string courseId, string slideId, int hintId)
		{
			var userId = User.Identity.GetUserId();
			await slideHintRepo.AddHint(userId, hintId, courseId, slideId);
			return "success";
		}

		[HttpPost]
		[Authorize]
		public string GetHint(string courseId, string slideId)
		{
			var userId = User.Identity.GetUserId();
			var answer = slideHintRepo.GetHint(userId, courseId, slideId);
			return answer;
		}

		[HttpPost]
		[Authorize]
		public string GetAllQuestions(string courseName)
		{
			var questions = userQuestionsRepo.GetAllQuestions(courseName);
			return Server.HtmlEncode(questions);
		}

		[HttpPost]
		[Authorize]
		public async Task<JsonResult> LikeSolution(int solutionId)
		{
			var res = await solutionsRepo.Like(solutionId, User.Identity.GetUserId());
			return Json(new { likesCount = res.Item1, liked = res.Item2 });
		}

		private string NormalizeString(string s)
		{
			return s.LineEndingsToUnixStyle().Trim();
		}

		private async Task<RunSolutionResult> CheckSolution(ExerciseSlide exerciseSlide, string code, int slideIndex)
		{
			var solution = exerciseSlide.Solution.BuildSolution(code);
			var submition = await executionService.Submit(solution, "");
			var output = submition.Output + "\n" + submition.StdErr;
			var isRightAnswer = NormalizeString(output).Equals(NormalizeString(exerciseSlide.ExpectedOutput));
			return new RunSolutionResult
			{
				CompilationError = submition.CompilationError,
				IsRightAnswer = isRightAnswer,
				ExpectedOutput = exerciseSlide.ExpectedOutput,
				ActualOutput = output
			};
		}

		private async Task SaveUserSolution(string courseId, string slideId, string code, string compilationError, string output,
			bool isRightAnswer)
		{
			await solutionsRepo.AddUserSolution(
				courseId, slideId,
				code, isRightAnswer, compilationError, output,
				User.Identity.GetUserId());
		}


		private static string GetUserCode(Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}

		public async Task VisitSlide(string courseId, string slideId)
		{
			await visitersRepo.AddVisiter(courseId, slideId, User.Identity.GetUserId());
		}

		public async Task<string> SubmitQuiz(string courseId, string slideIndex, string answer)
		{
			var intSlideIndex = int.Parse(slideIndex);
			var answers = answer.Split('*').Select(x => x.Split('_').Take(2).ToList()).GroupBy(x => x[0]);
			var incorrectQuizzes = new List<string>();
			var course = courseManager.GetCourse(courseId);
			foreach (var ans in answers)
			{
				var quizInfo = GetQuizInfo(course, intSlideIndex, ans);
				foreach (var quizInfoForDb in quizInfo)
				{
					await
						userQuizzesRepo.AddUserQuiz(courseId, quizInfoForDb.IsRightAnswer, quizInfoForDb.ItemId, quizInfoForDb.QuizId,
							course.Slides[intSlideIndex].Id, quizInfoForDb.Text, User.Identity.GetUserId());
					if (!quizInfoForDb.IsRightAnswer)
						incorrectQuizzes.Add(ans.Key);
				}
			}
			return string.Join("*", incorrectQuizzes);
		}

		private IEnumerable<QuizInfoForDb> GetQuizInfo(Course course, int slideIndex, IGrouping<string, List<string>> answer)
		{
			var slide = course.Slides[slideIndex] as QuizSlide;
			if (slide == null)
				throw new Exception("Error in func 'GetSlideUsingId' or in 'QuizSlideId'");
			var block = slide.Quiz.Blocks[int.Parse(answer.Key)];
			var data = answer.ToList();
			if (block is FillInBlock)
				return CreateQuizInfoForDb(block as FillInBlock, data);
			if (block is ChoiceBlock)
				return CreateQuizInfoForDb(block as ChoiceBlock, answer, data);
			return CreateQuizInfoForDb(block as IsTrueBlock, data);
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(IsTrueBlock isTrueBlock, IEnumerable<List<string>> data)
		{
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = isTrueBlock.Id,
					ItemId = null,
					IsRightAnswer = isTrueBlock.Answer.ToString() == data.First()[1],
					Text = null
				}
			};
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(ChoiceBlock choiseBlock, IEnumerable<List<string>> answer, List<List<string>> data)
		{
			if (choiseBlock.Multiple)
			{
				var ans = answer.ToList()
					.Select(x => new QuizInfoForDb {QuizId = choiseBlock.Id, IsRightAnswer = false, ItemId = x[1], Text = null}).ToList();
				var correctItems = new HashSet<string>(choiseBlock.Items.Where(x => x.IsCorrect).Select(x => x.Id));
				var ansItem = new HashSet<string>(ans.Select(x => x.ItemId));
				var count = ansItem.Count(correctItems.Contains);
				if (count != correctItems.Count || count != ansItem.Count) return ans;
				foreach (var info in ans)
					info.IsRightAnswer = true;
				return ans;
			}
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = choiseBlock.Id,
					ItemId = choiseBlock.Items[int.Parse(data.First()[1])].Id,
					IsRightAnswer = choiseBlock.Items[int.Parse(data.First()[1])].IsCorrect,
					Text = null
				}
			};
		}

		private IEnumerable<QuizInfoForDb> CreateQuizInfoForDb(FillInBlock fillInBlock, IEnumerable<List<string>> data)
		{
			return new List<QuizInfoForDb>
			{
				new QuizInfoForDb
				{
					QuizId = fillInBlock.Id,
					ItemId = null,
					IsRightAnswer = fillInBlock.Regexes.Any(regex => Regex.IsMatch(data.First()[1], regex)),
					Text = data.First()[1]
				}
			};
		}
	}

	public class QuizInfoForDb
	{
		public string QuizId;
		public string ItemId;
		public string Text;
		public bool IsRightAnswer;
	}
}