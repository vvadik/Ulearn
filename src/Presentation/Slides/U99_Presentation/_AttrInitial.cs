using System;

namespace Presentation.Slides.U99_Presentation
{
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