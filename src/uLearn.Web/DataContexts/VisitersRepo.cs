using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class VisitersRepo
	{
		private readonly ULearnDb db;

		public VisitersRepo() : this(new ULearnDb())
		{
			
		}

		public VisitersRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task AddVisiter(string courseId, int slideIndex, string userId)
		{
			if (db.Visiters.Any(x => x.CourseId == courseId && x.UserId == userId && x.SlideId == slideIndex))
				return;
			db.Visiters.Add(new Visiters
			{
				UserId = userId,
				CourseId = courseId,
				SlideId = slideIndex
			});
			await db.SaveChangesAsync();
		}

		public int GetVisitersCount(int slideId, string courseId)
		{
			return db.Visiters.Count(x => x.SlideId == slideId && x.CourseId == courseId);
		}

		public bool IsUserVisit(string courseId, int slideId, string userId)
		{
			return db.Visiters.Any(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId);
		}
	}
}