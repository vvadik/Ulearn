using System;
using uLearn;

namespace Presentation.Slides.U99_Presentation
{
	[Slide("Attributes", "{EB939293-4AD4-4937-B863-B78A99CCF679}")]
	class S010_Attributes
	{
		/*
		Аттрибуты:
		*/

		public class ExampleSlideAttribute : Attribute
		{
			public string Title { get; set; }
			public string Guid { get; set; }

			public ExampleSlideAttribute(string title, string guid)
			{
				Title = title;
				Guid = guid;
			}
		}
	}
}
