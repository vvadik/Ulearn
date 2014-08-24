using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.U00_Presentation
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
