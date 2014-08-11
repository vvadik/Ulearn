using System;
using uLearn.Quizes;

namespace uLearn
{
	public class QuizInfoModel
	{
		public CoursePageModel CourseModel {get; set;}
		public QuizBlock CurrentBlock { get; set;}
		public int BlockIndex { get; set; }
		public bool Passed { get; set; }

		public QuizInfoModel(CoursePageModel model, QuizBlock currentBlock, int index, bool passed)
		{
			CourseModel = model;
			CurrentBlock = currentBlock;
			BlockIndex = index;
			Passed = passed;
		}
	}

}
