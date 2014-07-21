using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Provider;
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
	}
}