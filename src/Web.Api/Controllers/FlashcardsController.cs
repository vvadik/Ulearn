using System;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.Flashcards;
using Database.Repos.Users;
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
		[HttpGet("{courseId}/flashcards/stat")]
		public ActionResult<FlashCardsStatResponse> FlashcardsStat([FromQuery] Guid? unitId = null)
		{
			return new FlashCardsStatResponse();
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
		[HttpGet("{courseId}/flashcards-info")]
		public ActionResult<FlashcardInfoList> FlashcardsInfo()
		{
			return new FlashcardInfoList();
		}

		/// <summary>
		/// Изменить оценку для флеш-карты
		/// </summary>
		[HttpPut("{courseId}/flashcards/{flashcardId}/status")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Status([FromRoute] string courseId, [FromRoute] string flashcardId, [FromBody] Score score)
		{
			return NoContent();
			var course = courseManager.FindCourse(courseId);
			if (course is null)
				return BadRequest($"course with id {courseId} does not exist");
			
			var unit = course.Units.Find(x => x.GetFlashcardById(flashcardId) != null);
			if (unit is null) 
				return BadRequest($"flashcard with id {flashcardId} does not exist");

			await usersFlashcardsVisitsRepo.AddFlashcardVisit(UserId, courseId, unit.Id, flashcardId, score, DateTime.Now);
			return NoContent();
		}
	}
}