using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.Flashcards;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.FlashCards;
using Score = Database.Models.Score;

namespace Ulearn.Web.Api.Controllers
{
	public enum FlashcardOrder
	{
		Original,
		Smart
	}

	[Route("/courses")]
	public class FlashcardsController : BaseController
	{
		private IUsersFlashcardsVisitsRepo usersFlashcardsVisitsRepo;

		public FlashcardsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, IUsersFlashcardsVisitsRepo usersFlashcardsVisitsRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.usersFlashcardsVisitsRepo = usersFlashcardsVisitsRepo;
		}

		/// <summary>
		/// Статистика по последним результатам
		/// </summary>
		[Authorize]
		[HttpGet("{courseId}/flashcards/stat")]
		public async Task<ActionResult<FlashCardsStatResponse>> FlashcardsStat([FromRoute] string courseId, [FromQuery] Guid? unitId = null)
		{
			var course = courseManager.FindCourse(courseId);
			if (course == null)
				return BadRequest($"course with id {courseId} does not exist");
			if (unitId != null)
			{
				var unit = course.FindUnitById((Guid)unitId);
				if (unit == null)
				{
					return BadRequest($"unit with {unitId} does not exist");
				}
			}

			List<UserFlashcardsVisit> userFlashcardsVisits;
			if (unitId != null)
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, (Guid)unitId);
			}
			else
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId);
			}

			var flashCardsStatResponse = FormFlashCardsStatResponse(userFlashcardsVisits);

			return flashCardsStatResponse;
		}

		private FlashCardsStatResponse FormFlashCardsStatResponse(List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var scoreResponse = new ScoreResponse();
			foreach (var flashcardVisit in userFlashcardsVisits)
			{
				switch (flashcardVisit.Score)
				{
					case Score.NotViewed:
						scoreResponse.NotViewed++;
						break;
					case Score.One:
						scoreResponse.One++;
						break;
					case Score.Two:
						scoreResponse.Two++;
						break;
					case Score.Three:
						scoreResponse.Three++;
						break;
					case Score.Four:
						scoreResponse.Four++;
						break;
					case Score.Five:
						scoreResponse.Five++;
						break;
				}
			}

			return new FlashCardsStatResponse() { ScoreResponse = scoreResponse, Total = userFlashcardsVisits.Count };
		}

		/// <summary>
		/// Коллекция карточек
		/// </summary>
		[HttpGet("{courseId}/flashcards")]
		public ActionResult<FlashCardsList> Flashcards([FromQuery] int count, [FromQuery] Guid unitId, [FromQuery] string status, [FromQuery] FlashcardOrder flashcardOrder)
		{
			return new FlashCardsList();
		}

		/// <summary>
		/// Информация о всех карточках по курсу
		/// </summary>
		[Authorize]
		[HttpGet("{courseId}/flashcards-info")]
		public async Task<ActionResult<FlashcardInfoList>> FlashcardsInfo([FromRoute] string courseId)
		{
			var course = courseManager.FindCourse(courseId);
			if (course is null)
				return BadRequest($"course with id {courseId} does not exist");
			var userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId);
			return FormFlashcardInfoList(userFlashcardsVisits);
		}

		private FlashcardInfoList FormFlashcardInfoList(List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var result = new FlashcardInfoList();
			foreach (var visit in userFlashcardsVisits)
			{
				var info = new FlashcardsInfo();
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
		public async Task<IActionResult> Status([FromRoute] string courseId, [FromRoute] string flashcardId, [FromBody] Score score)
		{
			var course = courseManager.FindCourse(courseId);
			if (course is null)
				return BadRequest($"course with id {courseId} does not exist");
			//todo проверка существования карточки
			//var unit = course.Units.Find(x => x.GetFlashcardById(flashcardId) != null);
			//if (unit is null)
			//	return BadRequest($"flashcard with id {flashcardId} does not exist");
			if ((int)score < 0 || (int)score > 5)
			{
				return BadRequest($"value {score} of score is invalid");
			}

			var unitId = new Guid("e1beb629-6f24-279a-3040-cf111f91e764");
			await usersFlashcardsVisitsRepo.AddFlashcardVisitAsync(UserId, courseId, unitId, flashcardId, score, DateTime.Now);
			return NoContent();
		}
	}
}