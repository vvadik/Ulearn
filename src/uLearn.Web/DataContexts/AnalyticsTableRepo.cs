using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

		public async void AddVisiter(string userId, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			table.Visiters.Add(new Visiter{UserId = userId});
			await db.SaveChangesAsync();
		}

		public async void AddMark(string userId, SlideMarks mark, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			table.Marks.Add(new SlideMark { UserId = userId, Mark = mark});
			await db.SaveChangesAsync();
		}

		public async void AddSolver(string userId, string key)
		{
			var table = db.AnalyticsTables.Find(key);
			table.Solvers.Add(new Solver { UserId = userId });
			await db.SaveChangesAsync();
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
			return !db.AnalyticsTables.Any(x => x.Id == key);
		}

		public string CreateKey(string courseName, string unitName, string slideTitle)
		{
			return courseName + "_" + unitName + "_" + slideTitle;
		}
	}
}