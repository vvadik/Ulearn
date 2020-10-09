using System;
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
		public List<UserExerciseSubmission> Submissions;

		public SlideRenderContext(string courseId, Slide slide, string baseUrl, bool removeHiddenBlocks,
			string videoAnnotationsGoogleDoc, IUrlHelper urlHelper,
			List<UserExerciseSubmission> submissions)
		{
			CourseId = courseId;
			Slide = slide;
			BaseUrl = baseUrl;
			RemoveHiddenBlocks = removeHiddenBlocks;
			VideoAnnotationsGoogleDoc = videoAnnotationsGoogleDoc;
			UrlHelper = urlHelper;
			Submissions = submissions;
		}
	}
}