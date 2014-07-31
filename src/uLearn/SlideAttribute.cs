using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class SlideAttribute : Attribute
	{
		public string Title { get; set; }
		public string Guid { get; set; }

		public SlideAttribute(string title, string guid)
		{
			Title = title;
			Guid = guid;
		}
	}
}