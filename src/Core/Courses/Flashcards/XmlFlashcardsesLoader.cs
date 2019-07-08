using System;
using System.Collections.Generic;
using System.IO;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides;

namespace Ulearn.Core.Courses.Flashcards
{
	public class XmlFlashcardsesLoader : IFlashcardsLoader
	{
		public List<Flashcards> Load(FlashcardLoadingContext context)
		{
			var fileContent = context.FlashcardsFile.ReadAllContent();
			throw new NotImplementedException();

		}
		
		public List<Flashcards> Load(byte[] fileContent, FlashcardLoadingContext context)
		{
			
			var flashcardsFile = context.FlashcardsFile ?? new FileInfo("<internal>");
			
			//var slideType = DetectSlideType(fileContent, slideFile.Name);

			/*var slide = (Slide)fileContent.DeserializeXml(slideType);

			slide.BuildUp(context);

			slide.Validate(context);

			return slide;*/
			throw new NotImplementedException();

		}
	}
}