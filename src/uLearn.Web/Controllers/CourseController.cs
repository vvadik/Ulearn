using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CourseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db = new ULearnDb();
		private readonly SlideHintRepo slideHintRepo = new SlideHintRepo();
		private readonly SlideRateRepo slideRateRepo = new SlideRateRepo();
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly UnitsRepo unitsRepo = new UnitsRepo();
		private readonly UserQuestionsRepo userQuestionsRepo = new UserQuestionsRepo();
		private readonly UserQuizzesRepo userQuizzesRepo = new UserQuizzesRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();

		public CourseController()
			: this(WebCourseManager.Instance)
		{
		}

		public CourseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[Authorize]
		public async Task<ActionResult> Slide(string courseId, int slideIndex = -1)
		{
			if (string.IsNullOrWhiteSpace(courseId)) return RedirectToAction("Index", "Home");
			var visibleUnits = unitsRepo.GetVisibleUnits(courseId, User);
			var model = await CreateCoursePageModel(courseId, slideIndex, visibleUnits);
			if (!visibleUnits.Contains(model.Slide.Info.UnitName))
				throw new Exception("Slide is hidden " + slideIndex);
			var exerciseSlide = model.Slide as ExerciseSlide;
			if (exerciseSlide != null)
				exerciseSlide.LikedHints = slideHintRepo.GetLikedHints(courseId, exerciseSlide.Id, User.Identity.GetUserId());
			var quizSlide = model.Slide as QuizSlide;
			if (quizSlide != null)
				foreach (var block in quizSlide.Quiz.Blocks.Where(x => x is ChoiceBlock).Where(x => ((ChoiceBlock) x).Shuffle))
					Shuffle(block);
			return View(model);
		}

		private void Shuffle(QuizBlock quizBlock)
		{
			var random = new Random();
			var choiceBlock = (ChoiceBlock) quizBlock;
			choiceBlock.Items = choiceBlock.Items.OrderBy(x => random.Next()).ToArray();
		}

		private int GetInitialIndexForStartup(string courseId, Course course, List<string> visibleUnits)
		{
			var userId = User.Identity.GetUserId();
			var lastVisitedSlide = 0;
			for (var index = 0; index < course.Slides.Length; index++)
			{
				var slide = course.Slides[index];
				if (!visibleUnits.Contains(slide.Info.UnitName)) continue;
				if (visitersRepo.IsUserVisit(courseId, slide.Id, userId))
					lastVisitedSlide = index;
				else
					return lastVisitedSlide;
			}
			return lastVisitedSlide;
		}

		[Authorize]
		[HttpGet]
		public ActionResult SelectGroup()
		{
			var groups = db.Users.Select(u => u.GroupName).Distinct().OrderBy(g => g).ToArray();
			return PartialView(groups);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult> SelectGroup(string groupName)
		{
			var userId = User.Identity.GetUserId();
			var user = db.Users.FirstOrDefault(x => x.Id == userId);
			if (user != null)
			{
				user.GroupName = groupName;
				await db.SaveChangesAsync();
				return Content("");
			}
			return HttpNotFound("User not found");
		}

		private async Task<CoursePageModel> CreateCoursePageModel(string courseId, int slideIndex, List<string> visibleUnits)
		{
			var course = courseManager.GetCourse(courseId);
			if (slideIndex == -1)
				slideIndex = GetInitialIndexForStartup(courseId, course, visibleUnits);
			var userId = User.Identity.GetUserId();
			var isFirstCourseVisit = !db.Visiters.Any(x => x.UserId == userId);
			await VisitSlide(courseId, course.Slides[slideIndex].Id);
			var model = new CoursePageModel
			{
				IsFirstCourseVisit = isFirstCourseVisit,
				CourseId = course.Id,
				CourseTitle = course.Title,
				Slide = course.Slides[slideIndex],
				LatestAcceptedSolution =
					solutionsRepo.FindLatestAcceptedSolution(courseId, course.Slides[slideIndex].Id, userId),
				Rate = GetRate(course.Id, course.Slides[slideIndex].Id),
				PassedQuiz = userQuizzesRepo.GetIdOfQuizPassedSlides(courseId, userId),
				AnswersToQuizes =
					userQuizzesRepo.GetAnswersForShowOnSlide(courseId, course.Slides[slideIndex] as QuizSlide,
						userId)
			};
			return model;
		}

		[Authorize]
		public ActionResult AcceptedSolutions(string courseId, int slideIndex = 0)
		{
			var userId = User.Identity.GetUserId();
			var course = courseManager.GetCourse(courseId);
			var slide = (ExerciseSlide)course.Slides[slideIndex];
			var isPassed = solutionsRepo.IsUserPassedTask(courseId, slide.Id, userId);
			var solutions = isPassed
				? solutionsRepo.GetAllAcceptedSolutions(courseId, slide.Id)
				: new List<AcceptedSolutionInfo>();
			foreach (var solution in solutions)
				solution.LikedAlready = solution.UsersWhoLike.Any(u => u == userId);
			var model = new AcceptedSolutionsPageModel
			{
				CourseId = courseId,
				CourseTitle = course.Title,
				Slide = slide,
				AcceptedSolutions = solutions
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateInput(false)]
		public async Task<string> AddQuestion(string courseId, string slideId, string question)
		{
			IIdentity user = User.Identity;
			var slide = courseManager.GetCourse(courseId).GetSlideById(slideId);
			await userQuestionsRepo.AddUserQuestion(question, slide.Title, user.GetUserId(), user.Name, slide.Info.UnitName, DateTime.Now);
			return "Success!";
		}

		[HttpPost]
		[Authorize]
		public async Task<string> ApplyRate(string courseId, string slideId, string rate)
		{
			var userId = User.Identity.GetUserId();
			var slideRate = (SlideRates) Enum.Parse(typeof (SlideRates), rate);
			return await slideRateRepo.AddRate(courseId, slideId, userId, slideRate);
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
		public string GetAllQuestions(string unitName)
		{
			var questions = userQuestionsRepo.GetAllQuestions(unitName);
			return Server.HtmlEncode(questions);
		}

		[Authorize]
		public ActionResult Questions(string unitName)
		{
			var questions = db.UserQuestions.Where(q => q.UnitName == unitName).ToList();
			return PartialView(questions);
		}

		[HttpPost]
		[Authorize]
		public async Task<JsonResult> LikeSolution(int solutionId)
		{
			var res = await solutionsRepo.Like(solutionId, User.Identity.GetUserId());
			return Json(new {likesCount = res.Item1, liked = res.Item2});
		}

		public async Task VisitSlide(string courseId, string slideId)
		{
			await visitersRepo.AddVisiter(courseId, slideId, User.Identity.GetUserId());
		}

		[Authorize(Roles = LmsRoles.Instructor)]
		public ActionResult InstructorNote(string courseId, string unitName)
		{
			InstructorNote instructorNote = courseManager.GetCourse(courseId).GetInstructorNote(unitName);
			return View(instructorNote);
		}
		}
}