using System.Collections.Generic;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Core.Courses.Slides;

namespace Ulearn.Web.Api.Controllers.Slides
{
	public class SlideRenderContext
	{
		public string CourseId;
		public Slide Slide;
		public string BaseUrl;
		public bool RemoveHiddenBlocks;
		public string VideoAnnotationsGoogleDoc;
		public IUrlHelper UrlHelper;
		[CanBeNull] public List<UserExerciseSubmission> Submissions;
		[CanBeNull] public List<ExerciseCodeReviewComment> CodeReviewComments;

		public SlideRenderContext(string courseId, Slide slide, string baseUrl, bool removeHiddenBlocks,
			string videoAnnotationsGoogleDoc, IUrlHelper urlHelper,
			[CanBeNull]List<UserExerciseSubmission> submissions, [CanBeNull]List<ExerciseCodeReviewComment> codeReviewComments)
		{
			CourseId = courseId;
			Slide = slide;
			BaseUrl = baseUrl;
			RemoveHiddenBlocks = removeHiddenBlocks;
			VideoAnnotationsGoogleDoc = videoAnnotationsGoogleDoc;
			UrlHelper = urlHelper;
			Submissions = submissions;
			CodeReviewComments = codeReviewComments;
		}
	}
}