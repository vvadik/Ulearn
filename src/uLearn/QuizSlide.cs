using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.Quizes;

namespace uLearn
{
	public class QuizSlide : Slide
	{
		public QuizSlide(SlideInfo slideInfo, Quiz quiz)
			: base(new SlideBlock[0], slideInfo, quiz.Title, quiz.Id)
		{
			Quiz = quiz;
		}

		public Quiz Quiz { get; set; }
	}
}
