using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.Owin.Security.Provider;
using NUnit.Framework;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class AnalyticsTableRepo
	{
		private readonly ULearnDb db;

		public AnalyticsTableRepo() : this(new ULearnDb())
		{

		}

		public AnalyticsTableRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task<string> AddVisiter(string userId, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			if (table == null)
			{
				await AddNewTable(key);
				table = db.AnalyticsTables.Find(key);
			}
			if (table.Visiters == null)
				table.Visiters = new List<Visiter>();
			if (table.Visiters.Any(x => x.UserId == userId))
				return "already yet";
			table.Visiters.Add(new Visiter { UserId = userId });
			await db.SaveChangesAsync();
			return "success";
		}


		public async Task<string> AddMark(string userId, SlideMarks mark, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			if (table != null)
			{
				if (table.Marks == null)
					table.Marks = new List<SlideMark>();
				if (table.Marks.Any(x => x.UserId == userId))
					table.Marks.First(x => x.UserId == userId).Mark = mark;
				else
					table.Marks.Add(new SlideMark { UserId = userId, Mark = mark });

			}
			else
			{
				await AddNewTable(key);
				table = db.AnalyticsTables.Find(key);
				table.Marks.Add(new SlideMark { UserId = userId, Mark = mark });
			}
			await db.SaveChangesAsync();
			return "success!";
		}

		public async Task<string> AddHint(string userId, int hintId, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			if (table != null)
			{
				if (table.Hints == null)
					table.Hints = new List<Hint>();
				if (table.Hints.Any(x => x.UserId == userId && x.HintId == hintId))
					return "Added yet";
				table.Hints.Add(new Hint { UserId = userId, HintId = hintId });
				await db.SaveChangesAsync();
			}
			return "success";
		}

		public string GetHint(string key, string userId)
		{
			var table = db.AnalyticsTables.Find(key);
			if (table != null)
			{
				if (table.Hints != null)
				{
					var hints = table.Hints.Where(x => x.UserId == userId).ToList();
					if (hints.Count != 0)
						return string.Join(" ", hints.Select(x => x.HintId).ToList());
				}
					
			}
			return null;
		}

		private async Task<string> AddNewTable(string key)
		{
			db.AnalyticsTables.Add(new AnalyticsTable
			{
				Id = key,
				Marks = new List<SlideMark>(),
				Solvers = new List<Solver>(),
				Visiters = new List<Visiter>()
			});
			await db.SaveChangesAsync();
			return "success!";
		}

		public async Task<string> AddSolver(string userId, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			if (table == null)
			{
				await AddNewTable(key);
				table = db.AnalyticsTables.Find(key);
			}
			if (table.Solvers == null)
				table.Solvers = new List<Solver>();
			if (table.Solvers.Any(x => x.UserId == userId))
				return "already yet";
			table.Solvers.Add(new Solver { UserId = userId });
			await db.SaveChangesAsync();
			return "success";
		}

		public int GetSolversCount(string key)
		{
			return IsCollectionEmpty(key) ? 0 : db.AnalyticsTables.Where(x => x.Id == key).Select(x => x.Solvers.Count).Single();
		}

		public int GetVisitersCount(string key)
		{
			return IsCollectionEmpty(key) ? 0 : db.AnalyticsTables.Where(x => x.Id == key).Select(x => x.Visiters.Count).Single();
		}

		public Marks GetMarks(string key)
		{
			var marks = new Marks();
			if (IsCollectionEmpty(key))
				return marks;
			foreach (var table in db.AnalyticsTables.Single(x => x.Id == key).Marks)
			{
				if (table.Mark == SlideMarks.Good)
					marks.AddGood();
				if (table.Mark == SlideMarks.NotUnderstand)
					marks.AddNotUnderstand();
				if (table.Mark == SlideMarks.NotWatched)
					marks.AddNotWatched();
				if (table.Mark == SlideMarks.Trivial)
					marks.AddTrivial();
			}
			return marks;
		}

		private bool IsCollectionEmpty(string key)
		{
			return db.AnalyticsTables.Find(key) == null;
		}

		public string CreateKey(string courseName, string unitName, string slideTitle)
		{
			return courseName + "_" + unitName + "_" + slideTitle;
		}

		public string FindMark(string courseName, string unitName, string slideTitle, string userId)
		{
			var key = CreateKey(courseName, unitName, slideTitle);
			var ans = db.AnalyticsTables.Where(x => x.Id == key).ToList();
			if (ans.Count == 0)
				return null;
			var mark = ans.Select(x => x.Marks.FirstOrDefault(y => y.UserId == userId)).FirstOrDefault();
			return mark == null ? null : mark.Mark.ToString();
		}

		public Dictionary<string, UserStatsInfo> CreateUsersStats(Course course, int slideIndex)
		{
			var ans = new Dictionary<string, UserStatsInfo>();
			foreach (var slide in course.Slides)
			{
				var key = CreateKey(
					course.Title,
					slide.Info.UnitName,
					slide.Title);
				var table = db.AnalyticsTables.Find(key);
				if (table == null)
					continue;
				var localKey = key.Split('_')[1];
				
				foreach (var solver in table.Solvers)
				{
					if (!ans.ContainsKey(solver.UserId))
						ans[solver.UserId] = new UserStatsInfo();
					if (!ans[solver.UserId].PercentAcceptedSolutionsPerUnit.ContainsKey(localKey))
						ans[solver.UserId].PercentAcceptedSolutionsPerUnit[localKey] = new UnitStat();
					ans[solver.UserId].PercentAcceptedSolutionsPerUnit[localKey].AddCount();
				}
			}
			return ans;
		}

		public Dictionary<string, PersonalStatisticsInSlide> CreatePersonalStatistics(string userId, Course course)
		{
			var ans = new Dictionary<string, PersonalStatisticsInSlide>();
			foreach (var slide in course.Slides)
			{
				var key = CreateKey(
					course.Title,
					slide.Info.UnitName,
					slide.Title);
				var stat = db.AnalyticsTables.Find(key);
				if (stat == null)
					continue;
				var personalStatistics = new PersonalStatisticsInSlide
				{
					IsNotExercise = (slide as ExerciseSlide) == null,
					IsSolved = stat.Solvers.Any(x => x.UserId == userId),
					IsVisited = stat.Visiters.Any(x => x.UserId == userId),
					UserMark = ConvertMarkToPrettyString(stat.Marks.FirstOrDefault(x => x.UserId == userId))
				};
				ans[key.Remove(0, key.IndexOf('_') + 1).Replace("_", ": ")] = personalStatistics;
			}
			return ans;
		}

		private string ConvertMarkToPrettyString(SlideMark slideMark)
		{
			return slideMark == null
				? null
				: slideMark.Mark == SlideMarks.Good
					? "Хорошо"
					: slideMark.Mark == SlideMarks.NotUnderstand
						? "Не понял"
						: slideMark.Mark == SlideMarks.Trivial
							? "Тривиально"
							: null;
		}
	}
}