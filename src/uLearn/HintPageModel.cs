using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class HintPageModel
	{
		public HintWithLikeButton[] Hints { get; set; }
	}

	public class HintWithLikeButton
	{
		public int HintId { get; set; }
		public bool IsLiked { get; set; }
		public string CourseId { get; set; }
		public string SlideId { get; set; }
		public string Hint { get; set; }
	}
}
