﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Core.Courses.Manager;
using Ulearn.Web.Api.Models.Responses.Review;

namespace Ulearn.Web.Api.Controllers.Review
{
	[Route("/favourite-reviews")]
	public class FavouriteReviewsController : BaseController
	{
		public FavouriteReviewsController(
			ICourseStorage courseStorage,
			UlearnDb db,
			IUsersRepo usersRepo)
			: base(courseStorage, db, usersRepo)
		{
		}

		[HttpGet]
		[Authorize(Policy = "Instructors")]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "You don't have access to view submissions")]
		public async Task<ActionResult<FavouriteReviewsResponse>> GetFavouriteReviews([FromQuery] string courseId, [FromQuery] Guid slideId)
		{
			var reviews = new List<FavouriteReview>
			{
				new()
				{
					Id = 0,
					Text = "**bold** __italic__ ```code```",
					IsFavourite = false,
					RenderedText = "<b>bold</b> <i>italic</i> <code>code</code>",
					UseCount = 10
				},
				new()
				{
					Id = 1,
					Text = "Ой! Наш робот нашёл решения других студентов, подозрительно похожие на ваше. Так может быть, если вы позаимствовали части программы, взяли их из открытых источников либо сами поделились своим кодом.Выполняйте задания самостоятельно.",
					IsFavourite = false,
					RenderedText = "Ой! Наш робот нашёл решения других студентов, подозрительно похожие на ваше. Так может быть, если вы позаимствовали части программы, взяли их из открытых источников либо сами поделились своим кодом.Выполняйте задания самостоятельно.",
					UseCount = 100
				},
				new()
				{
					Id = 2,
					Text = "Так делать не стоит из-за сложности в O(N^3). Есть более оптимизированные алгоритмы",
					IsFavourite = true,
					RenderedText = "Так делать не стоит из-за сложности в O(N^3). Есть более оптимизированные алгоритмы",
					UseCount = 5
				}
			};
			
			await Task.Delay(100); //todo MOCKED
			
			return new FavouriteReviewsResponse
			{
				FavouriteReviews = reviews
			};
		}
	}
}