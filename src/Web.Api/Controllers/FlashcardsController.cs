using System;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/courses")]
	public class FlashcardsController : BaseController
	{
		public FlashcardsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo)
			: base(logger, courseManager, db, usersRepo)
		{
		}

		/// <summary>
		/// Статистика по последним результатам
		/// </summary>
		/// <param name="unitId"></param>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards/stat")]
		public ActionResult<FlashCardsStat> FlashcardsStat([FromQuery] Guid? unitId = null)
		{
			return new FlashCardsStat();
		}

		/// <summary>
		/// Коллекция карточек
		/// </summary>
		/// <param name="count"></param>
		/// <param name="unitId"></param>
		/// <param name="status"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards")]
		public ActionResult<FlashCardsList> Flashcards([FromQuery] int count, [FromQuery] Guid unitId, [FromQuery] string status, [FromQuery] Order order)
		{
			return new FlashCardsList();
		}

		/// <summary>
		/// Информация о всех карточках по курсу
		/// </summary>
		/// <returns></returns>
		[HttpGet("{courseId}/flashcards-info")]
		public ActionResult<FlashcardInfoList> FlashcardsInfo()
		{
			return new FlashcardInfoList();
		}

		/// <summary>
		/// Изменить оценку для флеш-карты
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpPut("{courseId}/flashcards/{flashcardId}/status")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Status([FromBody] Enum status)
		{
			return NoContent();
		}
	}
}