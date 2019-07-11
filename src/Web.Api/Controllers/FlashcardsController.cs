using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.Flashcards;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Serilog;
using Ulearn.Core;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Flashcards;
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

			List<UserFlashcardsVisit> userFlashcardsVisits;
			int totalFlascardsCount;

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

				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId, unitId.Value);
				totalFlascardsCount = unit.Flashcards.Count;
			}

			else
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId);
				totalFlascardsCount = course.Units.Sum(x => x.Flashcards.Count);
			}

			var scoreResponse = ToFlashCardsStatResponse(userFlashcardsVisits);

			return new FlashcardsStatResponse() { ScoreResponse = scoreResponse, TotalFlashcardsCount = totalFlascardsCount };
		}

		private ScoreResponse ToFlashCardsStatResponse(List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var scoreResponse = new ScoreResponse();
			foreach (var flashcardVisit in userFlashcardsVisits)
			{
				switch (flashcardVisit.Score)
				{
					case Rate.NotRated:
						scoreResponse.NotRated++;
						break;
					case Rate.Rate1:
						scoreResponse.Rate1++;
						break;
					case Rate.Rate2:
						scoreResponse.Rate2++;
						break;
					case Rate.Rate3:
						scoreResponse.Rate3++;
						break;
					case Rate.Rate4:
						scoreResponse.Rate4++;
						break;
					case Rate.Rate5:
						scoreResponse.Rate5++;
						break;
				}
			}

			return scoreResponse;
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
		public async Task<ActionResult<FlashcardsResponse>> Flashcards([FromRoute] string courseId, [FromQuery] int? count, [FromQuery] Guid? unitId, [FromQuery] Rate? rate, [FromQuery] FlashcardOrder flashcardOrder = FlashcardOrder.Smart)
		{
			courseId = courseId.ToLower();
			List<UserFlashcardsVisit> userFlashcardsVisits;
			List<Flashcard> flashcards;

			var course = courseManager.FindCourse(courseId);
			if (course is null)
			{
				return BadRequest($"course with id {courseId} does not exist");
			}

			if (unitId != null)
			{
				var unit = course.FindUnitById(unitId.Value);
				if (unit is null)
				{
					return BadRequest($"unit with {unitId} does not exist");
				}

				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId, unitId.Value);
				flashcards = unit.Flashcards;
			}
			else
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId);
				flashcards = course.Units.SelectMany(x => x.Flashcards).ToList();
			}

			var userFlashcardsVisitsDictionary = userFlashcardsVisits.ToDictionary(x => x.FlashcardId);
			var flashcardsResponse = new FlashcardsResponse();
			foreach (var flashcard in flashcards)
			{
				if (rate != null &&
					rate.Value != Rate.NotRated &&
					(!userFlashcardsVisitsDictionary.ContainsKey(flashcard.Id) ||
					userFlashcardsVisitsDictionary[flashcard.Id].Score == Rate.NotRated ||
					userFlashcardsVisitsDictionary[flashcard.Id].Score != rate.Value))
					continue;
				if (rate != null &&
					rate.Value == Rate.NotRated &&
					userFlashcardsVisitsDictionary.ContainsKey(flashcard.Id) &&
					userFlashcardsVisitsDictionary[flashcard.Id].Score != Rate.NotRated)
					continue;
				var question = new StringBuilder();
				var answer = new StringBuilder();
				foreach (var answerBlock in flashcard.Answer.Blocks)
				{
					if (answerBlock.GetType() == typeof(MarkdownBlock))
					{
						answer.Append(answerBlock.TryGetText().RenderMarkdown());
					}
				}

				foreach (var questionBlock in flashcard.Question.Blocks)
				{
					if (questionBlock.GetType() == typeof(MarkdownBlock))
					{
						question.Append(questionBlock.TryGetText().RenderMarkdown());
					}
				}

				Rate rateResponse;
				if (userFlashcardsVisitsDictionary.ContainsKey(flashcard.Id))
				{
					rateResponse = userFlashcardsVisitsDictionary[flashcard.Id].Score;
				}
				else
				{
					rateResponse = Rate.NotRated;
				}

				Guid unitIdResponse;
				if (unitId == null)
					unitIdResponse = Guid.Empty;
				else
					unitIdResponse = unitId.Value;
				var flashcardResponse = new FlashcardResponse() { Answer = answer.ToString(), Question = question.ToString(), Rate = rateResponse, Id = flashcard.Id, UnitId = unitIdResponse };
				flashcardsResponse.Flashcards.Add(flashcardResponse);
			}

			if (count != null && flashcardsResponse.Flashcards.Count > count)
			{
				flashcardsResponse.Flashcards = flashcardsResponse.Flashcards.Take(count.Value).ToList();
			}

			return flashcardsResponse;
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
		public async Task<IActionResult> Status([FromRoute] string courseId, [FromRoute] string flashcardId, [FromBody] Rate rate)
		{
			courseId = courseId.ToLower();
			var course = courseManager.FindCourse(courseId);
			if (course is null)
				return BadRequest($"course with id {courseId} does not exist");
			//todo проверка существования карточки
			//var unit = course.Units.Find(x => x.GetFlashcardById(flashcardId) != null);
			//if (unit is null)
			//	return BadRequest($"flashcard with id {flashcardId} does not exist");
			if ((int)rate < 0 || (int)rate > 5)
			{
				return BadRequest($"value {rate} of score is invalid");
			}

			var unitId = new Guid("e1beb629-6f24-279a-3040-cf111f91e764");
			await usersFlashcardsVisitsRepo.AddFlashcardVisitAsync(UserId, courseId, unitId, flashcardId, rate, DateTime.Now);
			return NoContent();
		}
	}
}