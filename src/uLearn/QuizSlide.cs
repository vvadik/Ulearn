using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class QuizSlide : Slide
	{
		public QuizSlide(IEnumerable<SlideBlock> blocks, SlideInfo slideInfo, Quiz quiz)
			: base(blocks, slideInfo, quiz.Title, quiz.Id)
		{
			Quiz = quiz;
		}

		public Quiz Quiz { get; set; }
	}
}
