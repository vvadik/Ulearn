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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Flashcards;
using Score = Database.Models.Score;

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

		public FlashcardsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo, IUsersFlashcardsVisitsRepo usersFlashcardsVisitsRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.usersFlashcardsVisitsRepo = usersFlashcardsVisitsRepo;
		}

		/// <summary>
		/// Статистика по оценкам карточек пользователя 
		/// </summary>
		[Authorize]
		[HttpGet("{courseId}/flashcards/stat")]
		public async Task<ActionResult<FlashcardsStatResponse>> FlashcardsStat([FromRoute] string courseId, [FromQuery] Guid? unitId = null)
		{
			courseId = courseId.ToLower();
			var course = courseManager.FindCourse(courseId);
			if (course == null)
				return BadRequest($"course with id {courseId} does not exist");
			if (unitId != null)
			{
				var unit = course.FindUnitById(unitId.Value);
				if (unit == null)
				{
					return BadRequest($"unit with {unitId} does not exist");
				}
			}

			List<UserFlashcardsVisit> userFlashcardsVisits;
			if (unitId != null)
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId, unitId.Value);
			}
			else
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId);
			}

			var flashCardsStatResponse = ToFlashCardsStatResponse(userFlashcardsVisits);

			return flashCardsStatResponse;
		}

		private FlashcardsStatResponse ToFlashCardsStatResponse(List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var scoreResponse = new ScoreResponse();
			foreach (var flashcardVisit in userFlashcardsVisits)
			{
				switch (flashcardVisit.Rate)
				{
					case Score.NotRated:
						scoreResponse.NotRated++;
						break;
					case Score.Rate1:
						scoreResponse.Rate1++;
						break;
					case Score.Rate2:
						scoreResponse.Rate2++;
						break;
					case Score.Rate3:
						scoreResponse.Rate3++;
						break;
					case Score.Rate4:
						scoreResponse.Rate4++;
						break;
					case Score.Rate5:
						scoreResponse.Rate5++;
						break;
				}
			}

			return new FlashcardsStatResponse() { ScoreResponse = scoreResponse, TotalFlashcardsCount = userFlashcardsVisits.Count };
		}

		/// <summary>
		/// Коллекция объектов карточек с оценками
		/// </summary>
		/// <param name="count">
		/// Если не указать, то придут все карточки, соответствующие остальным фильтрам
		/// </param>
		/// <param name="unitId"></param>
		/// <param name="rate"></param>
		/// <param name="flashcardOrder">
		/// original - карточки в исходном порядке
		/// smart - карточки в порядке, определяемом логикой показывания карточек
		/// </param>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards")]
		public ActionResult<FlashcardsResponse> Flashcards([FromQuery] int? count, [FromQuery] Guid? unitId, [FromQuery] Score? rate, [FromQuery] FlashcardOrder flashcardOrder = FlashcardOrder.Smart)
		{
			return new FlashcardsResponse();
		}

		/// <summary>
		/// Информация о всех карточках по курсу
		/// </summary>
		[Authorize]
		[HttpGet("{courseId}/flashcards-info")]
		public async Task<ActionResult<FlashcardInfoResponse>> FlashcardsInfo([FromRoute] string courseId)
		{
			courseId = courseId.ToLower();

			var course = courseManager.FindCourse(courseId);
			if (course is null)
				return BadRequest($"course with id {courseId} does not exist");
			var userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId);
			return ToFlashcardInfoResponse(userFlashcardsVisits);
		}

		private FlashcardInfoResponse ToFlashcardInfoResponse(List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var result = new FlashcardInfoResponse();
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
			courseId = courseId.ToLower();
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