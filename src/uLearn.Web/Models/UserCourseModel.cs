using System.Collections.Generic;
using System.Linq;
using uLearn.Quizes;
using uLearn.Web.DataContexts;

namespace uLearn.Web.Models
{
	public class UserCourseModel
	{

		public UserCourseModel(Course course, ApplicationUser user, ULearnDb db)
		{
			Course = course;
			User = user;

			var visits = db.Visiters.Where(v => v.UserId == user.Id && v.CourseId == course.Id).GroupBy(v => v.SlideId).ToDictionary(g => g.Key, g => g.FirstOrDefault());
			var unitResults = new Dictionary<string, UserCourseUnitModel>();
			foreach (var slide in Course.Slides)
			{
				var unit = slide.Info.UnitName;
				if (!unitResults.ContainsKey(unit))
					unitResults.Add(unit, new UserCourseUnitModel
					{
						UnitName = unit,
						SlideVisits = new ProgressModel(),
						Exercises = new ProgressModel(),
						Quizes = new ProgressModel(),
						Total = new ProgressModel()
					});

				var res = unitResults[unit];
				var isVisited = visits.ContainsKey(slide.Id);
				var isPassed = isVisited && visits[slide.Id].IsPassed;
				var score = isPassed ? visits[slide.Id].Score : 0;

				res.SlideVisits.Total++;
				res.Total.Total += slide.MaxScore;
				res.Total.Earned += score;

				if (isVisited)
					res.SlideVisits.Earned++;

				if (slide is ExerciseSlide)
				{
					res.Exercises.Total += slide.MaxScore;
					res.Exercises.Earned += score;
				}

				if (slide is QuizSlide)
				{
					res.Quizes.Total += slide.MaxScore;
					res.Quizes.Earned += score;
				}
			}

			Units = course.GetUnits().Select(unitName => unitResults[unitName]).ToArray();
			Total = new UserCourseUnitModel
			{
				Total = new ProgressModel(),
				Exercises = new ProgressModel(),
				SlideVisits = new ProgressModel(),
				Quizes = new ProgressModel()
			};
			foreach (var result in Units)
			{
				Total.Total.Add(result.Total);
				Total.Exercises.Add(result.Exercises);
				Total.SlideVisits.Add(result.SlideVisits);
				Total.Quizes.Add(result.Quizes);
			}
		}

		public ApplicationUser User { get; set; }
		public Course Course;
		public UserCourseUnitModel[] Units;
		public UserCourseUnitModel Total;
	}


	public class UserCourseUnitModel
	{
		public string UnitName;
		public ProgressModel Total;
		public ProgressModel Exercises;
		public ProgressModel SlideVisits;
		public ProgressModel Quizes;
	}

	public class ProgressModel
	{
		public int Earned;
		public int Total;

		public decimal? Progress
		{
			get { return Total == 0 ? null : (decimal?)Earned / Total; }
		}

		public override string ToString()
		{
			if (Progress.HasValue)
				return Progress.Value.ToString("0%");
			return "—";
		}

		public void Add(ProgressModel progress)
		{
			Earned += progress.Earned;
			Total += progress.Total;
		}
	}
}