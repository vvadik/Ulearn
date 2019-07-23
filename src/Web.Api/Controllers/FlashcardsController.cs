using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.CourseRoles;
using Database.Repos.Flashcards;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings;
using Serilog;
using Ulearn.Core;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Flashcards;
using Ulearn.Core.Courses.Units;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Flashcards;

namespace Ulearn.Web.Api.Controllers
{
	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum FlashcardOrder
	{
		Smart,
		Original
	}

	[Route("/courses")]
	public class FlashcardsController : BaseController
	{
		private readonly IUsersFlashcardsVisitsRepo usersFlashcardsVisitsRepo;
		private readonly IUserFlashcardsUnlockingRepo userFlashcardsUnlockingRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;


		public FlashcardsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			IUsersFlashcardsVisitsRepo usersFlashcardsVisitsRepo,
			IUserFlashcardsUnlockingRepo userFlashcardsUnlockingRepo,
			ICourseRolesRepo courseRolesRepo, IGroupAccessesRepo groupAccessesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.usersFlashcardsVisitsRepo = usersFlashcardsVisitsRepo;
			this.userFlashcardsUnlockingRepo = userFlashcardsUnlockingRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.groupAccessesRepo = groupAccessesRepo;
		}

		/// <summary>
		/// Коллекция объектов флешкарт с оценками, сгруппированных по модулям по курсу
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards-by-units")]
		public async Task<ActionResult<FlashcardResponseByUnits>> Flashcards([FromRoute] Course course)
		{
			var userFlashcardsVisitsByCourse = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, course.Id);
			var flashcardResponseByUnits = new FlashcardResponseByUnits();
			foreach (var unit in course.Units)
			{
				var unitFlashcardsResponse = new UnitFlashcardsResponse();
				var unitFlashcards = unit.Flashcards;
				if (unitFlashcards.Count == 0)
					continue;

				var flashcardResponsesEnumerable = GetFlashcardResponses(course, unitFlashcards, userFlashcardsVisitsByCourse);

				unitFlashcardsResponse.Flashcards.AddRange(flashcardResponsesEnumerable);
				unitFlashcardsResponse.UnitId = unit.Id;
				unitFlashcardsResponse.UnitTitle = unit.Title;
				unitFlashcardsResponse.Unlocked = await IsUnlocked(course, unit);
				flashcardResponseByUnits.Units.Add(unitFlashcardsResponse);
			}

			return flashcardResponseByUnits;
		}

		private async Task<bool> IsUnlocked(Course course, Unit unit)
		{
			var unlocking = await userFlashcardsUnlockingRepo.GetUserFlashcardsUnlocking(UserId, course, unit);
			if (unlocking != null)
				return true;
			var flashcards = unit.Flashcards;
			var userVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, course.Id, unit.Id);
			var userVisitsDict = new Dictionary<string, UserFlashcardsVisit>();
			foreach (var visit in userVisits)
			{
				userVisitsDict[visit.FlashcardId] = visit;
			}

			if (!flashcards.All(x => userVisitsDict.ContainsKey(x.Id)))
				return false;
			await userFlashcardsUnlockingRepo.AddUserFlashcardsUnlocking(UserId, course, unit);
			return true;
		}

		private IEnumerable<FlashcardResponse> GetFlashcardResponses(Course course, List<Flashcard> flashcards, List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var userFlashcardsVisitsDictionary = GetFlashcardsUsersVisitsDictionaryIncludingNotRated(userFlashcardsVisits, flashcards);
			var tLasts = GetFlashcardsTLasts(flashcards, userFlashcardsVisits);

			foreach (var flashcard in flashcards)
			{
				var question = GetRenderedContent(flashcard.Question.Blocks);
				var answer = GetRenderedContent(flashcard.Answer.Blocks);

				var rateResponse = userFlashcardsVisitsDictionary.TryGetValue(flashcard.Id, out var visit) ? visit.Rate : Rate.NotRated;

				var unit = course.Units.FirstOrDefault(x => x.GetFlashcardById(flashcard.Id) != default(Flashcard));

				var unitIdResponse = unit.Id;
				var tLast = tLasts[flashcard.Id];

				var flashcardResponse = new FlashcardResponse { Answer = answer, Question = question, Rate = rateResponse, Id = flashcard.Id, UnitId = unitIdResponse, UnitTitle = unit.Title, TheorySlidesIds = flashcard.TheorySlidesIds, FlashcardsRatesCountAfterLastRepeat = tLast };
				yield return flashcardResponse;
			}
		}

		private Dictionary<string, int> GetFlashcardsTLasts(List<Flashcard> flashcards, List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var result = new Dictionary<string, int>();
			foreach (var flashcard in flashcards)
			{
				result[flashcard.Id] = 0;
			}

			foreach (var visit in userFlashcardsVisits)
			{
				var id = visit.FlashcardId;
				foreach (var flashcard in flashcards)
				{
					result[flashcard.Id]++;
				}

				if (result.ContainsKey(id))
				{
					result[id] = 0;
				}
			}

			return result;
		}

		private static string GetRenderedContent(SlideBlock[] blocks)
		{
			var content = new StringBuilder();
			foreach (var block in blocks)
			{
				switch (block)
				{
					case MarkdownBlock markdownBlock:
						content.Append(markdownBlock.TryGetText().RenderMarkdown());
						break;
					case CodeBlock codeBlock:
					{
						content.Append($"\n<textarea class=\"code code-sample\" data-lang=\"{codeBlock.Language.GetName()}\">{codeBlock.Code}</textarea>");
						break;
					}

					case TexBlock texBlock:
						var lines = texBlock.TexLines.Select(x => $"<div class=\"tex\">{x.Trim()}</div>");
						content.Append(string.Join("\n", lines));
						break;
					default:
						content.Append(block.TryGetText());
						break;
				}
			}

			return content.ToString();
		}

		private Dictionary<string, UserFlashcardsVisit> GetFlashcardsUsersVisitsDictionaryIncludingNotRated(List<UserFlashcardsVisit> userFlashcardsVisits, List<Flashcard> flashcards)
		{
			var result = new Dictionary<string, UserFlashcardsVisit>();
			foreach (var userFlashcardsVisit in userFlashcardsVisits)
			{
				result[userFlashcardsVisit.FlashcardId] = userFlashcardsVisit;
			}

			foreach (var flashcard in flashcards)
			{
				if (!result.ContainsKey(flashcard.Id))
				{
					result[flashcard.Id] = new UserFlashcardsVisit { FlashcardId = flashcard.Id, Rate = Rate.NotRated };
				}
			}

			return result;
		}

		/// <summary>
		/// Изменить оценку для флеш-карты
		/// </summary>
		///
		[Authorize]
		[HttpPut("{courseId}/flashcards/{flashcardId}/status")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Status([FromRoute] Course course, [FromRoute] string flashcardId, [FromBody] Rate rate)
		{
			var unit = course.Units.FirstOrDefault(x => x.GetFlashcardById(flashcardId) != null);
			if (unit is null)
				return BadRequest($"flashcard with id {flashcardId} does not exist");
			await usersFlashcardsVisitsRepo.AddFlashcardVisitAsync(UserId, course.Id, unit.Id, flashcardId, rate, DateTime.Now);
			return NoContent();
		}

		/// <summary>
		/// Статистика по всем флеш-картам в курсе
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards/statistics")]
		public async Task<ActionResult<FlashcardsStatistics>> FlashcardsStatistics([FromRoute] Course course)
		{
			var hasUserAccessToCourse = await courseRolesRepo.HasUserAccessToCourseAsync(UserId, course.Id, CourseRoleType.Instructor);
			if (!hasUserAccessToCourse)
			{
				return BadRequest($"You don't have access to course with id {course.Id}");
			}

			var flashcardVisitsByCourse = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(course.Id);
			var flashcards = course.Units.SelectMany(x => x.Flashcards).ToList();
			var statistics = ToFlashcardsStatistics(flashcardVisitsByCourse, flashcards);

			return statistics;
		}

		private FlashcardsStatistics ToFlashcardsStatistics(List<UserFlashcardsVisit> userFlashcardsVisits, List<Flashcard> flashcards)
		{
			var result = new FlashcardsStatistics();

			var groupedByFlashcard = userFlashcardsVisits.GroupBy(x => x.FlashcardId).ToDictionary(x => x.Key);
			foreach (var flashcard in flashcards)
			{
				var flashcardStat = new FlashcardStatistic();

				flashcardStat.FlashcardId = flashcard.Id;
				if (groupedByFlashcard.TryGetValue(flashcard.Id, out var group))
				{
					foreach (var e in group)
					{
						flashcardStat.Statistics.Add(e.Rate);
					}

					flashcardStat.VisitCount = group.Count();
				}

				result.Statistics.Add(flashcardStat);
			}

			return result;
		}

		/// <summary>
		/// Статистика флеш-карт по пользователям доступных групп
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards/users-statistics")]
		public async Task<ActionResult<UserFlashcardStatisticResponse>> UserFlashcardStatistics([FromRoute] Course course)
		{
			var result = new UserFlashcardStatisticResponse();
			var groups = await groupAccessesRepo.GetAvailableForUserGroupsAsync(UserId, true);
			if (groups.Count == 0)
			{
				return BadRequest("You don't have access to any group in course");
			}

			var flashcards = course.Units.SelectMany(x => x.Flashcards).ToList();

			foreach (var group in groups)
			{
				foreach (var member in group.Members)
				{
					var userVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(member.UserId, course.Id);
					var flashcardStat = ToFlashcardsStatistics(userVisits, flashcards);
					var userStat = new UserFlashcardsStatistics
					{
						UserId = member.UserId,
						FlashcardsStatistics = flashcardStat,
						UserName = member.User.VisibleNameWithLastNameFirst,
						TotalFlashcardsVisits = flashcardStat.Statistics.Sum(x => x.VisitCount)
					};
					result.UsersFlashcardsStatistics.Add(userStat);
				}
			}

			return result;
		}
	}
}