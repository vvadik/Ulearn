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
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Flashcards;
using Ulearn.Core.Courses.Units;
using Ulearn.Web.Api.Models.Common;
using Ulearn.Web.Api.Models.Responses.Flashcards;
using Ulearn.Web.Api.Models.Responses.FlashCards;

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
		public async Task<ActionResult<FlashcardsStatResponse>> FlashcardsStat([FromRoute] Course course, [FromQuery] Guid? unitId = null)
		{
			var courseId = course.Id;
			List<UserFlashcardsVisit> userFlashcardsVisits;
			int totalFlashcardsCount;
			List<Flashcard> flashcards;

			if (unitId != null)
			{
				var unit = course.FindUnitById(unitId.Value);
				if (unit == null)
				{
					return BadRequest($"unit with {unitId} does not exist");
				}

				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId, unitId.Value);
				totalFlashcardsCount = unit.Flashcards.Count;
				flashcards = unit.Flashcards;
			}

			else
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, courseId);
				totalFlashcardsCount = course.Units.Sum(x => x.Flashcards.Count);
				flashcards = course.Units.SelectMany(x => x.Flashcards).ToList();
			}

			return ToFlashCardsStatResponse(userFlashcardsVisits, totalFlashcardsCount, flashcards);
		}

		private FlashcardsStatResponse ToFlashCardsStatResponse(List<UserFlashcardsVisit> userFlashcardsVisits, int totalFlashcardsCount, List<Flashcard> flashcards)
		{
			var flashcardsDict = flashcards.ToDictionary(x => x.Id);
			var scoreResponse = new TotalRateResponse();
			foreach (var flashcardVisit in userFlashcardsVisits)
			{
				if (!flashcardsDict.ContainsKey(flashcardVisit.FlashcardId))
					continue;
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

			var ratedCount = scoreResponse.Rate1 + scoreResponse.Rate2 + scoreResponse.Rate3 + scoreResponse.Rate4 + scoreResponse.Rate5;
			scoreResponse.NotRated = totalFlashcardsCount - ratedCount;

			return new FlashcardsStatResponse() { TotalRateResponse = scoreResponse, TotalFlashcardsCount = totalFlashcardsCount };
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
		public async Task<ActionResult<FlashcardsResponse>> Flashcards([FromRoute] Course course, [FromQuery] int? count, [FromQuery] Guid? unitId, [FromQuery] Rate? rate, [FromQuery] FlashcardOrder flashcardOrder = FlashcardOrder.Smart)
		{
			List<UserFlashcardsVisit> userFlashcardsVisits;
			List<Flashcard> flashcards;

			if (unitId != null)
			{
				var unit = course.FindUnitById(unitId.Value);
				if (unit is null)
				{
					return BadRequest($"unit with {unitId} does not exist");
				}

				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, course.Id, unitId.Value);
				flashcards = unit.Flashcards;
			}
			else
			{
				userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, course.Id);
				flashcards = course.Units.SelectMany(x => x.Flashcards).ToList();
			}

			var flashcardsResponse = new FlashcardsResponse();
			var flashcardResponsesIenumerable = GetFlashcardResponses(rate, unitId, course, flashcards, userFlashcardsVisits);
			if (count is null)
			{
				flashcardsResponse.Flashcards = flashcardResponsesIenumerable.ToList();
			}
			else
			{
				flashcardsResponse.Flashcards = flashcardResponsesIenumerable.Take(count.Value).ToList();
			}

			return flashcardsResponse;
		}

		private IEnumerable<FlashcardResponse> GetFlashcardResponses(Rate? rate, Guid? unitId, Course course, List<Flashcard> flashcards, List<UserFlashcardsVisit> userFlashcardsVisits)
		{
			var userFlashcardsVisitsDictionary = GetFlashcardsFullUsersVisitsDictionary(userFlashcardsVisits, flashcards);

			foreach (var flashcard in flashcards)
			{
				if (rate != null && userFlashcardsVisitsDictionary[flashcard.Id].Score != rate.Value)
					continue;

				var question = GetRenderedContent(flashcard.Question.Blocks);
				var answer = GetRenderedContent(flashcard.Answer.Blocks);

				var rateResponse = userFlashcardsVisitsDictionary.TryGetValue(flashcard.Id, out var visit) ? visit.Score : Rate.NotRated;
				Unit unit;
				if (unitId is null)
				{
					unit = course.Units.FirstOrDefault(x => x.GetFlashcardById(flashcard.Id) != default(Flashcard));
				}
				else
				{
					unit = course.FindUnitById(unitId.Value);
				}

				var unitIdResponse = unit.Id;
				var flashcardResponse = new FlashcardResponse { Answer = answer, Question = question, Rate = rateResponse, Id = flashcard.Id, UnitId = unitIdResponse, UnitTitle = unit.Title };
				yield return flashcardResponse;
			}
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
						content.Append($"\n<textarea class=\"code code-sample\" data-lang=\"{codeBlock.Language.GetName()}\" data-code=\"{codeBlock.Code}\"></textarea>");
						break;
					}

					case TexBlock texBlock:
						content.Append(texBlock.TryGetText().RenderTex());
						break;
					default:
						content.Append(block.TryGetText());
						break;
				}
			}

			return content.ToString();
		}

		private Dictionary<string, UserFlashcardsVisit> GetFlashcardsFullUsersVisitsDictionary(List<UserFlashcardsVisit> userFlashcardsVisits, List<Flashcard> flashcards)
		{
			var result = userFlashcardsVisits.ToDictionary(x => x.FlashcardId);
			foreach (var flashcard in flashcards)
			{
				if (!result.ContainsKey(flashcard.Id))
				{
					result[flashcard.Id] = new UserFlashcardsVisit { FlashcardId = flashcard.Id, Score = Rate.NotRated };
				}
			}

			return result;
		}

		/// <summary>
		/// Информация о всех карточках по курсу
		/// </summary>
		[Authorize]
		[HttpGet("{courseId}/flashcards-info")]
		public async Task<ActionResult<FlashcardInfoResponse>> FlashcardsInfo([FromRoute] Course course)
		{
			var info = new FlashcardInfoResponse();

			foreach (var unit in course.Units)
			{
				var userFlashcardsVisits = await usersFlashcardsVisitsRepo.GetUserFlashcardsVisitsAsync(UserId, course.Id, unit.Id);
				var unlocked = userFlashcardsVisits.All(x => x.Score == Rate.NotRated);
				var cardsCount = unit.Flashcards.Count;
				if (cardsCount != 0)
					info.Add(new FlashcardsUnitInfo { UnitId = unit.Id, UnitTitle = unit.Title, CardsCount = unit.Flashcards.Count, Unlocked = unlocked });
			}

			return info;
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
			var unit = course.Units.Find(x => x.GetFlashcardById(flashcardId) != null);
			if (unit is null)
				return BadRequest($"flashcard with id {flashcardId} does not exist");
			await usersFlashcardsVisitsRepo.AddFlashcardVisitAsync(UserId, course.Id, unit.Id, flashcardId, rate, DateTime.Now);
			return NoContent();
		}
	}
}